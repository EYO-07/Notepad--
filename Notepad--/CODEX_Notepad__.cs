// -- text marker highlight - tags example 
// {Notepad--T;red:ISSUE;yellow:DEPRECATED,TESTING,PLACEHOLDER;silver:FIXED,REVISION;cyan:TODO,>>>,<<<}
// {Notepad--T;Cyan:Inventory;Silver:Logic,Dialetic;lightgreen:Workflow} 
// {Notepad--T;magenta:methods,attributes,variables}
// {Notepad--T;lightgreen:debug, interception}
// {Notepad--H;1:silver;2:lightblue}

// -- search tokens 
// {Notepad--T;red:scintilla;blue:editor}
// {Notepad--S:scintilla}

// -- BEGIN 
// Codex Library in Magic Oriented Programming 
namespace Codex;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using static Codex.Transmutation;
using static Codex.Incantation;
using static Codex.Incantation_TREEVIEW;
using static Codex.Incantation_DIALOG;
using static Codex.Incantation_PANEL; 
using static Codex.Incantation_CONTEXTMENU; 
using static Codex.Incantation_TEXTBOX;
using static Codex.Incantation_TABS;
using static Codex.Incantation_EVENTS;
using static Codex.Conjuration;
using static Codex.Incantation_MOUSE;
// -- ambiguities
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using BorderStyle = System.Windows.Forms.BorderStyle;
using TabDrawMode = System.Windows.Forms.TabDrawMode;
using Timer = System.Windows.Forms.Timer; 

// ===================================== transmutation 
// ... data manipulation 
public static class Transmutation {
	// Standart input and output 
	public static void print(string text) {
		Console.WriteLine(text);
	}
	public static string input(string display) {
		Console.Write(display);
        return Console.ReadLine();
	} 
	
	// Conversion methods 
	public static float to_float(object obj) {
		return Convert.ToSingle(obj);
	}
	public static int to_int(object obj) {
		return Convert.ToInt32(obj);
	}
	public static double to_double(object obj) {
		return Convert.ToDouble(obj);
	}
	public static string to_string(object obj) {
		return obj?.ToString() ?? string.Empty;
	}

	// Save and Load
	public static void save(string filename, string content) {
        try
        {
            string tempPath = filename + ".tmp";
            File.WriteAllText(tempPath, content);       // Write to temp
            File.Copy(tempPath, filename, overwrite: true); // Atomic replacement
            File.Delete(tempPath);                      // Clean temp
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error saving file: " + ex.Message);
        }
    }
	public static string? load(string filename) {
        try
        {
            if (!File.Exists(filename))
                return null;

            return File.ReadAllText(filename);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error loading file: " + ex.Message);
            return null;
        }
    }
	public static T new_object_from_json<T>(string filename) where T : class, new() {
		try {
			string? data_json = load(filename);
			if (string.IsNullOrWhiteSpace(data_json)) return new T();
			T? obj = JsonSerializer.Deserialize<T>(data_json);
			return obj ?? new T();
		} catch {
			// Logar o erro pode ser útil aqui também
			return new T();
		}
	}
	
	// Filesystem 
	public static string get_exec_dir() {
        return AppContext.BaseDirectory;
    }
	public static string join(string dir, string filename) {
        return Path.Combine(dir, filename);
    }
	public static string get_dir(string fullfilename) {
        if (string.IsNullOrEmpty(fullfilename)) return null;
        return Path.GetDirectoryName(fullfilename);
    }
    public static string get_filename(string fullfilename) {
        if (string.IsNullOrEmpty(fullfilename)) return null;
        return Path.GetFileName(fullfilename);
    }
    public static string get_parent_dir(string fullpath) {
        if (string.IsNullOrEmpty(fullpath)) return null;
        var dir = Path.GetDirectoryName(fullpath);
        if (string.IsNullOrEmpty(dir)) return null;
        return Directory.GetParent(dir)?.FullName;
    }
    public static bool file_exists(string fullpath) {
        if (string.IsNullOrEmpty(fullpath)) return false;
        return File.Exists(fullpath);
    }
    public static bool is_dir(string path) {
        if (string.IsNullOrEmpty(path)) return false;
        if (!Directory.Exists(path)) return false;
        var attr = File.GetAttributes(path);
        return attr.HasFlag(FileAttributes.Directory);
    }
    public static bool is_file(string path) {
        if (string.IsNullOrEmpty(path)) return false;
        if (!File.Exists(path)) return false;
        var attr = File.GetAttributes(path);
        return !attr.HasFlag(FileAttributes.Directory);
    }
	public static List<string> get_drives() {
		return DriveInfo.GetDrives()
			.Where(d => d.IsReady)
			.Select(d => d.RootDirectory.FullName.TrimEnd('\\'))
			.ToList();
	}
	public static bool? is_file_size_at_least(string fullpath, int kilobytes) {
		try {
			var fileInfo = new FileInfo(fullpath);
			if (!fileInfo.Exists)
				return null;

			long sizeInBytes = fileInfo.Length;
			long thresholdInBytes = kilobytes * 1024L;

			return sizeInBytes >= thresholdInBytes;
		} catch {
			return null;
		}
	}
	public static string get_extension(string fullpath) {
		if (string.IsNullOrWhiteSpace(fullpath))
			return "";

		try {
			return Path.GetExtension(fullpath);
		} catch {
			return "";
		}
	}
	public static bool has_extension(List<string> paths, string ext) {
		if ( paths.Count == 0 ) return false; 
		foreach( var path in paths){
			if (get_extension(path) == ext) return true;
		}
		return false;
	}

	// List 
	public static T? Get<T>(this List<T> list, int index) {
		return (index >= 0 && index < list.Count) ? list[index] : default;
	}   

    // Regex 
    // TODO/TESTING 
//    public static IEnumerable<string> comma_separated_iterator(ref string content) {}
    // variables
	private static List<string> CODE_EXTS = new List<string>{
		".cs",
        ".csproj",
        ".user",
		".cpp",
		".c",
		".h",
		".hpp",
		".js",
		".ts",
		".py",
        ".pyw",
		".lua",
		".sql",
		".xml",
		".html",
		".htm",
		".json",
		".java",
        ".txt",
        ".ahk",
        ".bat",
        ".sh",
        ".ps1",
        ".ltx",
        ".script",
        ".md",
        ".tex"
	};
	// variables | methods 
    public static bool is_code_file(string filename_or_ext) {
		if ( string.IsNullOrWhiteSpace(filename_or_ext) ) return false;
		string ext = filename_or_ext;
		if (filename_or_ext.Contains(".")) ext = Path.GetExtension(filename_or_ext);
		ext = ext.ToLower();
		return CODE_EXTS.Contains(ext);
	}
}

// ===================================== incantation 
// ... graphical user interface and user interaction 
/* incantation 
1. new_ ; constructors 
2. add_ ; add child component 
3. on_ ; event listener setup 
*/

public static class Incantation {
	public static void register_icon(Form mainForm, string icon_name, string namespace_str) {
		var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(namespace_str+"."+icon_name+".ico");
        if (stream != null) mainForm.Icon = new Icon(stream);
	}
	
    // utils
	public static T? get_first<T>(Control parent) where T : Control {
		foreach (Control child in parent.Controls) {
			if (child is T match)
				return match;
		}
		return null;
	}
	public static List<T> get_list<T>(Control parent) where T : Control {
		List<T> result = new List<T>();
		foreach (Control child in parent.Controls) {
			if (child is T match) result.Add(match);
		}
		return result;
	}
	public static T? get_first_nested<T>(Control parent) where T : Control { // TESTING
        foreach (Control child in parent.Controls) {
            if (child is T match) return match;
            var nested = get_first_nested<T>(child);
            if (nested != null) return nested;
        }
        return null;
    }

    // Size and position
	public static void size(Form form, int width, int height) {
        form.Size = new Size(width, height);
    }
    public static void size(Form form, float width_screen_percentage, float height_screen_percentage) {
        Screen screen = Screen.FromControl(form);
        Rectangle bounds = screen.WorkingArea;

        int newWidth = (int)(bounds.Width * width_screen_percentage);
        int newHeight = (int)(bounds.Height * height_screen_percentage);

        form.Size = new Size(newWidth, newHeight);
    }
	public static void center(Form form) {
        Screen screen = Screen.FromControl(form); // Gets the screen where the form is
        Rectangle bounds = screen.WorkingArea;    // Excludes taskbar and docked items

        int x = bounds.X + (bounds.Width - form.Width) / 2;
        int y = bounds.Y + (bounds.Height - form.Height) / 2;

        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(x, y);
    }
	public static void set_as_vertical_flow_layout(TableLayoutPanel panel,List<float> percentages) {
		if (panel == null || percentages == null || percentages.Count == 0)
			return;

		panel.SuspendLayout();
		panel.Controls.Clear();
		panel.RowStyles.Clear();
		panel.ColumnStyles.Clear();
		panel.ColumnCount = 1;
		panel.RowCount = percentages.Count;
		panel.AutoSize = false;
		panel.Dock = DockStyle.Fill;
		panel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

		// Set the single column to 100% stretch
		panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

		for (int i = 0; i < percentages.Count; i++) {
			float p = percentages[i];
			if (p <= 0f) {
				// AutoSize
				panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			}
			else {
				panel.RowStyles.Add(new RowStyle(SizeType.Percent, p));
			}
		}

		panel.ResumeLayout();
	}
	public static async void animated_resize(Form form, int x, int y, int width, int height, int steps, int delayMs) {
        if (form == null) return;
        // Current form properties
        int startX = form.Location.X;
        int startY = form.Location.Y;
        int startWidth = form.Width;
        int startHeight = form.Height;
        // Calculate step increments
        double deltaX = (x - startX) / (double)steps;
        double deltaY = (y - startY) / (double)steps;
        double deltaWidth = (width - startWidth) / (double)steps;
        double deltaHeight = (height - startHeight) / (double)steps;
        // Perform animation
        for (int i = 1; i <= steps; i++) {
            // Calculate new values
            int newX = startX + (int)(deltaX * i);
            int newY = startY + (int)(deltaY * i);
            int newWidth = startWidth + (int)(deltaWidth * i);
            int newHeight = startHeight + (int)(deltaHeight * i);
            // Update form properties on the UI thread
            form.Invoke((Action)(() => {
                form.Location = new System.Drawing.Point(newX, newY);
                form.Size = new System.Drawing.Size(newWidth, newHeight);
            }));
            // Delay for smooth animation
            await Task.Delay(delayMs);
        }
        // Ensure final position and size are exact
        form.Invoke((Action)(() =>
        {
            form.Location = new System.Drawing.Point(x, y);
            form.Size = new System.Drawing.Size(width, height);
        }));
    }
	public static void animated_resize(Form form, int x, int y, int width, int height){
		animated_resize(form, x, y, width, height, 20, 5);
	}
	public static void animated_resize(Form form, Rectangle rect) {
		if (form == null || rect.IsEmpty) return;
		int x = rect.X;       
        int y = rect.Y;       
        int width = rect.Width;   
        int height = rect.Height; 
		animated_resize(form, x,y, width, height);
	}
	public static void animated_resize(Form form, Rectangle rect, int steps, int delayMs) {
		if (form == null || rect.IsEmpty) return;
		int x = rect.X;       
        int y = rect.Y;       
        int width = rect.Width;   
        int height = rect.Height; 
		animated_resize(form, x,y, width, height, steps, delayMs);
	}
	public static Rectangle get_dock_rect(Form form, float w_scr_perc, float h_scr_perc, string pos) {
		// Returns a Rectangle for docking the form to a screen edge based on width and height percentages 
		// pos: "north", "south", "east", or "west" for docking position
		// w_scr_perc: Width as a percentage of the screen's width (0.0 to 1.0)
		// h_scr_perc: Height as a percentage of the screen's height (0.0 to 1.0)
		
        // Validate inputs
        if (form == null) throw new ArgumentNullException(nameof(form));
        if (w_scr_perc < 0 || w_scr_perc > 1 || h_scr_perc < 0 || h_scr_perc > 1)
            throw new ArgumentOutOfRangeException("Width and height percentages must be between 0.0 and 1.0");
        if (string.IsNullOrEmpty(pos)) throw new ArgumentNullException(nameof(pos));
        // Get the screen where the form is located
        Screen screen = Screen.FromControl(form);
        Rectangle workingArea = screen.WorkingArea;
        // Calculate dimensions based on percentages
        int width = (int)(workingArea.Width * w_scr_perc);
        int height = (int)(workingArea.Height * h_scr_perc);
        // Initialize position
        int x = 0;
        int y = 0;
        // Determine position based on docking
        switch (pos.ToLower())
        {
            case "north":
                x = workingArea.X + (workingArea.Width - width) / 2; // Center horizontally
                y = workingArea.Y; // Top edge
                break;
            case "south":
                x = workingArea.X + (workingArea.Width - width) / 2; // Center horizontally
                y = workingArea.Y + workingArea.Height - height; // Bottom edge
                break;
            case "east":
                x = workingArea.X + workingArea.Width - width; // Right edge
                y = workingArea.Y + (workingArea.Height - height) / 2; // Center vertically
                break;
            case "west":
                x = workingArea.X; // Left edge
                y = workingArea.Y + (workingArea.Height - height) / 2; // Center vertically
                break;
            default:
                throw new ArgumentException("Position must be 'north', 'south', 'east', or 'west'", nameof(pos));
        }
        // Return the calculated Rectangle
        return new Rectangle(x, y, width, height);
    }
	public static Rectangle get_centered_relative_to_screen_rect(Form form, float wScrPerc, float hScrPerc) {
        // Validate input percentages
        wScrPerc = Math.Clamp(wScrPerc, 0.1f, 1.0f);
        hScrPerc = Math.Clamp(hScrPerc, 0.1f, 1.0f);
        // Get the screen where the form is primarily located 
        Screen screen = Screen.FromControl(form);
        // Calculate rectangle dimensions
        int width = (int)(screen.WorkingArea.Width * wScrPerc);
        int height = (int)(screen.WorkingArea.Height * hScrPerc);
        // Calculate position to center the rectangle
        int x = screen.WorkingArea.X + (screen.WorkingArea.Width - width) / 2;
        int y = screen.WorkingArea.Y + (screen.WorkingArea.Height - height) / 2;
        return new Rectangle(x, y, width, height);
    }
    private static readonly Dictionary<Form, System.Windows.Forms.Timer> animationTimers = new();
	private static readonly Dictionary<Form, bool> dock_active = new();
	private static readonly float collapsedPerc = 0.1f;
	public static void set_as_dock_window(Form form, string dock, float focus_screen_perc) {
		// F12 toggles between docked and free mode.
		// Dock options: "north", "south", "east", "west"
		form.KeyPreview = true; // Ensure form gets key events
		dock_active[form] = false;
		Rectangle screen = Screen.FromControl(form).WorkingArea;
		int expandedSize = (dock == "north" || dock == "south")
			? (int)(screen.Height * focus_screen_perc)
			: (int)(screen.Width * focus_screen_perc);
		int collapsedSize = (dock == "north" || dock == "south")
			? (int)(screen.Height * collapsedPerc)
			: (int)(screen.Width * collapsedPerc);

		// Animate expand/collapse
		void Animate(bool expand) {
			if (!dock_active[form]) return;

			form.TopMost = true;
			form.FormBorderStyle = FormBorderStyle.None;

			// 🔧 Re-evaluate screen now, based on actual form position
			Rectangle screen = Screen.FromControl(form).WorkingArea;

			// Set starting dock position
			switch (dock.ToLower()) {
				case "north":
					form.Bounds = new Rectangle(screen.Left, screen.Top, screen.Width, collapsedSize);
					break;
				case "south":
					form.Bounds = new Rectangle(screen.Left, screen.Bottom - collapsedSize, screen.Width, collapsedSize);
					break;
				case "west":
					form.Bounds = new Rectangle(screen.Left, screen.Top, collapsedSize, screen.Height);
					break;
				case "east":
					form.Bounds = new Rectangle(screen.Right - collapsedSize, screen.Top, collapsedSize, screen.Height);
					break;
			}

			if (animationTimers.ContainsKey(form))
				animationTimers[form].Stop();

			var timer = new System.Windows.Forms.Timer { Interval = 15 };
			animationTimers[form] = timer;

			timer.Tick += (s, e) => {
				var bounds = form.Bounds;
				bool done = false;

				switch (dock.ToLower()) {
					case "north":
						int newH_N = expand ? bounds.Height + 20 : bounds.Height - 20;
						newH_N = Math.Clamp(newH_N, collapsedSize, expandedSize);
						form.Bounds = new Rectangle(screen.Left, screen.Top, screen.Width, newH_N);
						done = (newH_N == expandedSize && expand) || (newH_N == collapsedSize && !expand);
						break;

					case "south":
						int newH_S = expand ? bounds.Height + 20 : bounds.Height - 20;
						newH_S = Math.Clamp(newH_S, collapsedSize, expandedSize);
						form.Bounds = new Rectangle(screen.Left, screen.Bottom - newH_S, screen.Width, newH_S);
						done = (newH_S == expandedSize && expand) || (newH_S == collapsedSize && !expand);
						break;

					case "west":
						int newW_W = expand ? bounds.Width + 20 : bounds.Width - 20;
						newW_W = Math.Clamp(newW_W, collapsedSize, expandedSize);
						form.Bounds = new Rectangle(screen.Left, screen.Top, newW_W, screen.Height);
						done = (newW_W == expandedSize && expand) || (newW_W == collapsedSize && !expand);
						break;

					case "east":
						int newW_E = expand ? bounds.Width + 20 : bounds.Width - 20;
						newW_E = Math.Clamp(newW_E, collapsedSize, expandedSize);
						form.Bounds = new Rectangle(screen.Right - newW_E, screen.Top, newW_E, screen.Height);
						done = (newW_E == expandedSize && expand) || (newW_E == collapsedSize && !expand);
						break;
				}

				if (done) timer.Stop();
			};

			timer.Start();
		}

		// Use form activation instead of focus
		form.Activated += (s, e) => Animate(true);
		form.Deactivate += (s, e) => Animate(false);
		form.KeyDown += (s, e) => {
			if (e.KeyCode == Keys.F12) {
				if (dock_active[form]) {
					dock_active[form] = false;
					form.FormBorderStyle = FormBorderStyle.Sizable;
					form.TopMost = false;
					form.Bounds = new Rectangle(screen.Width / 4, screen.Height / 4, 800, 600);
				} else {
					dock_active[form] = true; 
					MessageBox.Show("Docking Activated");
				}
			}
		};
	}
    
    // Themed
	public static void hide_titlebar(Form form) {
		form.SuspendLayout();
		form.Visible = false;
		form.FormBorderStyle = FormBorderStyle.None;
		form.Visible = true;
		form.ResumeLayout();
	}
	public static void show_titlebar(Form form) {
		form.SuspendLayout();
		form.Visible = false;
		form.FormBorderStyle = FormBorderStyle.Sizable;
		form.Visible = true;
		form.ResumeLayout();
	}
	public static void apply_theme_recursive(Form form, Color background, Color foreground)	{
		if (form == null) return;

		form.BackColor = background;
		form.ForeColor = foreground;

		apply_theme_recursive((Control)form, background, foreground);
	}
	public static void apply_theme_recursive(Control control, Color background, Color foreground) {
		if (control == null) return;

		control.BackColor = background;
		control.ForeColor = foreground;

		foreach (Control child in control.Controls)
		{
			apply_theme_recursive(child, background, foreground);
		}
	}
	public static Color rgb(int r, int g, int b) {
		return Color.FromArgb(r,g,b);
	}
	
	public static void border_color_on_focus_change(Control control, Color focused, Color unfocused) {}
	
    public static void Center(Control ctrl, Control target) {
        
    }

	// tootip
	public static ToolTip add_tooltip(Control control, string text) {
        ToolTip tooltip = new ToolTip();

        tooltip.AutoPopDelay = 5000;
        tooltip.InitialDelay = 1000;
        tooltip.ReshowDelay = 500;
        tooltip.ShowAlways = true;

        tooltip.SetToolTip(control, text);

        return tooltip;
    }
	
}

public static class Incantation_EVENTS {
    // Events 
	public static void key_shortcut(Control control, string modifiers, string key, Action action) {
		if (control == null || action == null) return;
		if (!Enum.TryParse<Keys>(key, true, out Keys parsedKey)) return;
        key_shortcut(control, modifiers, parsedKey, action);
	}
    public static void key_shortcut(Control control, string modifiers, Keys key, Action action) {
		if (control == null || action == null) return;
		if (control is Form form) form.KeyPreview = true;
		var required = modifiers?.ToLower().Split('+', StringSplitOptions.RemoveEmptyEntries)
					   ?? Array.Empty<string>();
		bool ctrl  = required.Contains("ctrl");
		bool alt   = required.Contains("alt");
		bool shift = required.Contains("shift");

		control.KeyDown += (sender, e) =>{
			bool modifiersMatch =
				e.Control == ctrl &&
				e.Alt == alt &&
				e.Shift == shift;
			if (modifiersMatch && e.KeyCode == key) {
				action();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		};
	}

	private static Dictionary<Control, DateTime> _lastClickTimes = new();
	public static void on_double_click(Control control, Action action) {
		if (control == null || action == null) return;

		control.MouseDown += (sender, e) => {
			if (e.Button != MouseButtons.Left) return;

			DateTime now = DateTime.Now;

			if (_lastClickTimes.TryGetValue(control, out DateTime lastClick)) {
				double diff = (now - lastClick).TotalMilliseconds;
				if (diff <= SystemInformation.DoubleClickTime) {
					action.Invoke();
				}
			}

			_lastClickTimes[control] = now;
		};
	}
	
	[DllImport("user32.dll")]
	private static extern bool ReleaseCapture();
	[DllImport("user32.dll")]
	private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
	private const int WM_NCLBUTTONDOWN = 0xA1;
	private const int HTCAPTION = 0x2;
	public static void drag_window(Control control) {
		control.MouseDown += (sender, e) =>
		{
			Form? form = control.FindForm();
			if (form == null) return;
			if (form.WindowState == FormWindowState.Maximized) return;

			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(form.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
			}
		};
	}

    // -- 
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(Keys vKey);
    public static void OnTick(Control ctrl, Action<object, EventArgs> action, int milliseconds) {
        if (ctrl == null) throw new ArgumentNullException(nameof(ctrl));
        if (action == null) throw new ArgumentNullException(nameof(action));
        var timer = new Timer { Interval = milliseconds };
        timer.Tick += (s, e) => action(s, e);
        ctrl.HandleCreated += (s, e) => timer.Start();
        ctrl.Disposed += (s, e) => timer.Stop();
    }
    public static bool IsKeyDown(Keys key) {
        return (GetAsyncKeyState(key) & 0x8000) != 0;
    }

    [DllImport("user32.dll")] 
    private static extern IntPtr GetForegroundWindow();
    public static bool is_form_active(Form form) {
        return form.Handle == GetForegroundWindow();
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);
    private static readonly IntPtr HWND_TOP = new IntPtr(0);
    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;
    public static void bring_window_to_top(Form form) {
        try {
            SetWindowPos(form.Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        } catch (Exception ex) {}
    }
    public static void bring_window_to_least(Form form) {
        try {
            SetWindowPos(form.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        } catch (Exception ex) {}
    }
    
}

public static class Incantation_MOUSE {
    // mouse 
    [DllImport("user32.dll")] private static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out Point lpPoint);
    public static void screen_absolute_mouse_move(int x, int y) {
        SetCursorPos(x, y);
    }
    public static void window_absolute_mouse_move(Form form, int x, int y) {
        Point screenPoint = form.PointToScreen( new Point(x,y) );
        SetCursorPos(screenPoint.X, screenPoint.Y);
    }
    public static void relative_mouse_move(int dx, int dy) {
        if (GetCursorPos(out Point currentPos)) SetCursorPos(currentPos.X + dx, currentPos.Y + dy);
    } 
    
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, UIntPtr dwExtraInfo);
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP   = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP   = 0x0010;
    private const uint MOUSEEVENTF_WHEEL = 0x0800;
    public static void mouse_LButtonDown() {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
    }
    public static void mouse_LButtonUp() {
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
    }
    public static void mouse_LeftClick() {
        mouse_LButtonDown();
        mouse_LButtonUp();
    }
    public static void mouse_RButtonDown() {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
    }
    public static void mouse_RButtonUp() {
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
    }
    public static void mouse_RightClick() {
        mouse_RButtonDown();
        mouse_RButtonUp();
    }
    public static void mouse_ScrollUp() {
        // Positive value scrolls up
        mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 120, UIntPtr.Zero);
    }
    public static void mouse_ScrollDown() {
        // Negative value scrolls down
        mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -120, UIntPtr.Zero);
    }
}

public static class Incantation_TABS {
    // Tabs 
	public static TabControl new_tabs() {
		var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Multiline = true, 
        };
        return tabControl;
	}
	public static TabPage? add_tab<T>(TabControl tabs, string title) where T : Control, new() {
		if (tabs == null || string.IsNullOrEmpty(title)) return null;
		try	{
			var tabPage = new TabPage(title);
			var control = new T
			{
				Dock = DockStyle.Fill
			};
			tabPage.Controls.Add(control);
			tabs.TabPages.Add(tabPage);
			return tabPage;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Error creating tab with {typeof(T).Name}: {ex.Message}");
			return null;
		}
	}
	public static TabPage? get_pointed_tab(TabControl tabs, Point location)	{
		for (int i = 0; i < tabs.TabPages.Count; i++)
		{
			Rectangle tabRect = tabs.GetTabRect(i);
			if (tabRect.Contains(location))
			{
				return tabs.TabPages[i];
			}
		}
		return null;
	}
	public static bool close_tab(TabControl tabs, TabPage page)	{
		if (tabs == null || page == null) return false;
		if (!tabs.TabPages.Contains(page)) return false;

		tabs.TabPages.Remove(page);
		page.Dispose(); // optional: free resources associated with the tab
		return true;
	}
	public static bool is_page_in_tabs(TabPage page, TabControl tabs) {
		if (page == null || tabs == null)
			return false;

		return tabs.TabPages.Contains(page);
	}	
}

public static class Incantation_TEXTBOX {
    // TextBox 
	public static TextBox new_multiline_text_box() {
        return new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = true,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 8),
        };
    }
	public static TextBox new_text_box() {
		return new TextBox
        {
		 	BorderStyle = BorderStyle.FixedSingle, 
			WordWrap = true,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 8)
        };
	}
	public static TabControl new_multiline_text_tabs() { // deprecated 
		// --> new_tabs() 
        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Multiline = true, // allows multiple rows of tabs if needed
        };
        return tabControl;
    }
    public static DarkTabControl new_multiline_text_dark_tabs() {
		var tabControl = new DarkTabControl
        {
            Dock = DockStyle.Fill,
            Multiline = true, // allows multiple rows of tabs if needed
        };
        return tabControl;
	}
	public static DarkTabControl new_dark_tabs() {
		var tabControl = new DarkTabControl
        {
            Dock = DockStyle.Fill,
            Multiline = true, // allows multiple rows of tabs if needed
        };
        return tabControl;
	}
	public static TabPage? add_multiline_text_tab(TabControl tabs, string title) {
		return add_multiline_text_tab(tabs, title, new Font("Consolas", 10) );
    }
	public static TabPage? add_multiline_text_tab(TabControl tabs, string title, Font font) {
        if (tabs == null) return null;

        var newTab = new TabPage(title);
        var textBox = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical,
            Font = font,
            AcceptsTab = true,
            AcceptsReturn = true,
            WordWrap = true,
        };

        newTab.Controls.Add(textBox);
        tabs.TabPages.Add(newTab);
		return newTab;
    }
	// >>>
	public static List<string> get_tokens(TextBox textBox) {
		return textBox.Text
			.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
			.ToList();
	}
	// <<<
}

public static class Incantation_CONTEXTMENU {
    // Context Menu 
    public static ToolStripMenuItem add_item(ContextMenuStrip menu, string text) {
        var menuItem = new ToolStripMenuItem(text);
        menu.Items.Add(menuItem);
        return menuItem;
    } 
    public static ToolStripMenuItem add_context_menu_item(Control ctrl, string text) {
        var menu = ctrl.ContextMenuStrip; 
        return add_item(menu, text);
    }
	public static ContextMenuStrip add_context_menu(Control? control, List<object> items) {
		var menu = new ContextMenuStrip();
		foreach (var item in items) {
			if (item is string text) {
				// Simple menu item with text
				var menuItem = new ToolStripMenuItem(text);
				menu.Items.Add(menuItem);
			} 
			else if (item is ToolStripMenuItem submenu) {
				// Already a submenu, just add it
				menu.Items.Add(submenu);
			}
			else if (item is (string subText, List<object> subItems)) {
				// Tuple: submenu label + submenu items (recursive)
				var subMenuItem = new ToolStripMenuItem(subText);
				foreach (var subItem in subItems) {
					if (subItem is string st)
						subMenuItem.DropDownItems.Add(new ToolStripMenuItem(st));
					else if (subItem is ToolStripMenuItem sti)
						subMenuItem.DropDownItems.Add(sti);
					// Can extend for more nested types
				}
				menu.Items.Add(subMenuItem);
			}
			// Extend with more types as needed
		}
		if (control != null) control.ContextMenuStrip = menu; 
		return menu;
	}
	public static ToolStripMenuItem new_submenu(string label, List<object> items) {
		var submenu = new ToolStripMenuItem(label);
		foreach (var item in items) {
			if (item is string text) {
				submenu.DropDownItems.Add(new ToolStripMenuItem(text));
			}
			else if (item is ToolStripMenuItem menuItem) {
				submenu.DropDownItems.Add(menuItem);
			}
			else if (item is (string subText, List<object> subItems)) {
				var nestedSub = new ToolStripMenuItem(subText);
				foreach (var subItem in subItems) {
					if (subItem is string st)
						nestedSub.DropDownItems.Add(new ToolStripMenuItem(st));
					else if (subItem is ToolStripMenuItem sti)
						nestedSub.DropDownItems.Add(sti);
				}
				submenu.DropDownItems.Add(nestedSub);
			}
		}
		return submenu;
	}
	public static ToolStripMenuItem? get(ContextMenuStrip menu, string text) {
		foreach (ToolStripItem item in menu.Items) {
			if (item is ToolStripMenuItem menuItem) {
				if (menuItem.Text.Contains(text)) return menuItem;
				// Recursive search in submenus
				var found = get(menuItem.DropDown, text);
				if (found != null)
					return found;
			}
		}
		return null;
	}
	public static ToolStripMenuItem? get(ToolStripDropDown dropdown, string text) {
		foreach (ToolStripItem item in dropdown.Items) {
			if (item is ToolStripMenuItem menuItem) {
				if (menuItem.Text.Contains(text)) return menuItem;
				var found = get(menuItem.DropDown, text);
				if (found != null)
					return found;
			}
		}
		return null;
	}
	public static void set_action(ContextMenuStrip menu, string text, EventHandler handler){
		var item = get(menu, text);
		if (item==null) return ;
		item.Click += handler; 
	}
}

public static class Incantation_PANEL {
    // Panel
	public static FlowLayoutPanel new_horizontal_panel(List<Control> list) {
        var panel = new FlowLayoutPanel {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        foreach (var control in list) {
            panel.Controls.Add(control);
        }
        return panel;
    }
    public static FlowLayoutPanel new_vertical_panel(List<Control> list) {
        var panel = new FlowLayoutPanel {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        foreach (var control in list) {
            panel.Controls.Add(control);
        }
        return panel;
    }
	public static List<Control> new_button_list(List<string> labels)	{
		var buttons = new List<Control>();
		foreach (var label in labels)
		{
			var btn = new Button
			{
				Text = label,
				AutoSize = true // Optional: automatically size button to text
			};
			buttons.Add(btn);
		}
		return buttons;
	}
	public static List<Control> new_dark_button_list(List<string> labels) {
		var buttons = new List<Control>();
		foreach (var label in labels)
		{
			var btn = new Button
			{
				Text = label,
				AutoSize = true,
				BackColor = Color.FromArgb(30, 30, 40),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Margin = new Padding(4),
				Padding = new Padding(6, 4, 6, 4)
			};
			btn.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 90);
			btn.FlatAppearance.BorderSize = 1;

			buttons.Add(btn);
		}
		return buttons;
	}
	public static SplitContainer new_horizontal_split(Control ctrl1, Control ctrl2) {
        var split = new SplitContainer {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            Panel1MinSize = 50,
            Panel2MinSize = 50,
            SplitterDistance = 150 // adjust as needed
        };

        split.Panel1.Controls.Add(ctrl1);
        split.Panel2.Controls.Add(ctrl2);

        ctrl1.Dock = DockStyle.Fill;
        ctrl2.Dock = DockStyle.Fill;

        return split;
    }
	public static SplitContainer new_vertical_split(Control ctrl1, Control ctrl2) {
        var split = new SplitContainer {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            Panel1MinSize = 30,
            Panel2MinSize = 30,
        };
        split.Panel1.Controls.Add(ctrl1);
        split.Panel2.Controls.Add(ctrl2);
        ctrl1.Dock = DockStyle.Fill;
        ctrl2.Dock = DockStyle.Fill;
        return split;
    }
    public static void set_splitter_distance(SplitContainer split, int percentage) {
        if (split == null) return;
        if (percentage < 5 || percentage > 95) return;
//            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 1 and 100.");
        // For vertical orientation, SplitterDistance is measured in pixels from the left edge
        if (split.Orientation == Orientation.Vertical) {
            int distance = (split.Width * percentage) / 100;
            split.SplitterDistance = distance;
        }
        else {
            // For horizontal orientation, SplitterDistance is measured in pixels from the top
            int distance = (split.Height * percentage) / 100;
            split.SplitterDistance = distance;
        }
    }

    public static int get_focused_panel(SplitContainer split) {
        if (split == null) return -1;

        if (split.Panel1.ContainsFocus) return 1;
        if (split.Panel2.ContainsFocus) return 2;

        return -1; // focus is outside this SplitContainer
    }


}

public static class Incantation_TREEVIEW {
	private static string dummy_node = "Loading...";
	public static TreeView new_dummy_tree(string display_text){
		var tree = new TreeView { Dock = DockStyle.Fill };
		string label = display_text; 
		var rootNode = new TreeNode {
			Text = label,
			Tag = null
		};
		tree.Nodes.Add(rootNode);
		return tree;
	}
	public static TreeView new_tree(string root) {
		bool isDir = Directory.Exists(root);
		var tree = new TreeView { Dock = DockStyle.Fill };
		string label = Path.GetFileName(root.TrimEnd('\\'));
		if (!isDir || string.IsNullOrWhiteSpace(label) ) label = root; 
		var rootNode = new TreeNode {
			Text = label,
			Tag = isDir ? root : null
		};
		if (isDir) rootNode.Nodes.Add(dummy_node);
		tree.Nodes.Add(rootNode);
		return tree;
	}
	public static DarkTreeView new_dark_tree(string root) {
		bool isDir = Directory.Exists(root);
		var tree = new DarkTreeView { Dock = DockStyle.Fill };
		string label = Path.GetFileName(root.TrimEnd('\\'));
		if (!isDir || string.IsNullOrWhiteSpace(label) ) label = root; 
		var rootNode = new TreeNode {
			Text = label,
			Tag = isDir ? root : null
		};
		if (isDir) rootNode.Nodes.Add(dummy_node);
		tree.Nodes.Add(rootNode);
		return tree;
	}
	public static MultiSelectTreeView new_multiselection_tree(string root){
		bool isDir = Directory.Exists(root);
		var tree = new MultiSelectTreeView { Dock = DockStyle.Fill };
		string label = Path.GetFileName(root.TrimEnd('\\'));
		if (!isDir || string.IsNullOrWhiteSpace(label) ) label = root; 
		var rootNode = new TreeNode {
			Text = label,
			Tag = isDir ? root : null
		};
		if (isDir) rootNode.Nodes.Add(dummy_node);
		tree.Nodes.Add(rootNode);
		return tree;
	}
	public static void join(TreeView master, TreeView tree)	{
		if (master == null || tree == null) return;
		if (master.Nodes.Count == 0 || tree.Nodes.Count == 0) return;

		// Get the first root node of each
		TreeNode masterRoot = master.Nodes[0];
		TreeNode childNode = (TreeNode)tree.Nodes[0].Clone();

		// Add the root of 'tree' as a child of the master root
		masterRoot.Nodes.Add(childNode);
	}
	public static List<TreeNode> get_toplevel_nodes(TreeView tree) {
		if (tree == null || tree.Nodes.Count == 0)
			return new List<TreeNode>();

		return tree.Nodes.Cast<TreeNode>().ToList();
	}
	public static List<TreeNode> get_toplevel_nodes_of_rootnode(TreeView tree){
		if (tree ==null) return new List<TreeNode>();
		if (tree.Nodes.Count == 0) return new List<TreeNode>();
		TreeNode root = tree.Nodes[0];
		if (root.Nodes.Count == 0) return new List<TreeNode>();
		return root.Nodes.Cast<TreeNode>().ToList();
	}
	public static void set_as_filesystem_tree(TreeView tree) {
		if (tree == null) return;
		tree.BeforeExpand += (sender, e) => {
			var node = e.Node;
			string? path = node.Tag as string;
			if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;
			// Refresh only if dummy node is present
			if (node.Nodes.Count == 1 && node.Nodes[0].Text == dummy_node)
			{
				refresh_filesystem_node(node);
			}
		};
	}
	public static void refresh_filesystem_node(TreeNode node) { // REVISION
		string? path = node.Tag as string;
		if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;
		clear_non_existent_filesystem_node_childs(node);
		try {
			// Add directories
			foreach (var dir in get_directories(path)) {
				if ( is_path_in_filesystem_node(dir, node) ) continue;
				try {
					string dirName = Path.GetFileName(dir);
					var dirNode = new TreeNode(dirName) { Tag = dir };
					try {
						if (get_directories(dir).Length > 0 || get_files(dir).Length > 0)
							dirNode.Nodes.Add(dummy_node);
					} catch { }
					node.Nodes.Add(dirNode);
				} catch { }
			}
			// Add files
			foreach (var file in get_files(path)) {
				if ( is_path_in_filesystem_node(file, node) ) continue;
				try {
					string fileName = Path.GetFileName(file);
					var fileNode = new TreeNode(fileName) { Tag = file };
					node.Nodes.Add(fileNode);
				} catch { }
			}
		}
		catch (Exception ex) {
			node.Nodes.Add(new TreeNode($"Error: {ex.Message}"));
		}
	}
	public static void clear_non_existent_filesystem_node_childs(TreeNode node) {
		List<TreeNode> childs_to_be_removed = new List<TreeNode>(); 
		string? parent_path = node.Tag as string;
		if (string.IsNullOrEmpty(parent_path) || !Directory.Exists(parent_path)) return;
		foreach(TreeNode child_node in node.Nodes) {
			string? child_path = child_node.Tag as string;
			if ( string.IsNullOrEmpty(child_path) ) { childs_to_be_removed.Add(child_node); continue; }
			if (!is_dir(child_path) && !is_file(child_path)) childs_to_be_removed.Add(child_node);
		}
		foreach(TreeNode child_node in childs_to_be_removed) {
			node.Nodes.Remove(child_node);
		}
	}
	private static bool verify_filters(string test, List<string> filters) {
		bool has_at_least_one = false;
		int pos_count = 0;
		bool dont_have_any = true;
		foreach(var s in filters) {
			if ( s == "" ) continue ; 
			if ( s.StartsWith("-") ) {
				if ( s.Length <= 1 ) continue ;
				if ( test.Contains(s.Substring(1,s.Length-1), StringComparison.OrdinalIgnoreCase) ) dont_have_any = false ;
			} else {
				pos_count++;
				if ( test.Contains(s, StringComparison.OrdinalIgnoreCase) ) has_at_least_one = true;
			}
		}
		if (pos_count>0 && !has_at_least_one) return false;
		if (!dont_have_any) return false;
		return true; 
	}
	public static Dictionary<string, bool> clear_filtered_filesystem_node_childs(TreeNode node, List<string> filters) {
		Dictionary<string, bool> result = new Dictionary<string, bool>();
		HashSet<TreeNode> childs_to_be_removed = new HashSet<TreeNode>(); 
		string? parent_path = node.Tag as string;
		if (string.IsNullOrEmpty(parent_path) || !Directory.Exists(parent_path)) return result;
		foreach(TreeNode child_node in node.Nodes) {
			string? child_path = child_node.Tag as string;
			if ( string.IsNullOrEmpty(child_path) ) { childs_to_be_removed.Add(child_node); continue; }
			if (!is_dir(child_path) && !is_file(child_path)) { childs_to_be_removed.Add(child_node); continue; }
			if ( is_file(child_path) ) {
				string name = Path.GetFileName(child_path);
				result[name] = verify_filters(name, filters);
				if (! result[name] ) childs_to_be_removed.Add(child_node);
			}
		}
		foreach(TreeNode child_node in childs_to_be_removed) {
			node.Nodes.Remove(child_node);
		}
		return result;
	}
	public static bool is_path_in_filesystem_node(string path, TreeNode node) {
		// return null if path is invalid 
		if ( string.IsNullOrEmpty(path) ) return false;
		if (!is_dir(path) && !is_file(path)) return false;
		foreach(TreeNode child in node.Nodes) {
			string? child_path = child.Tag as string;
			if (string.IsNullOrEmpty(child_path)) continue;
			if ( string.Equals(path, child_path, StringComparison.OrdinalIgnoreCase) ) {
				return true;
			}
		}
		return false;
	}
	public static void filter_filesystem_node(TreeNode node, List<string> filters) {
		if (node == null) return; 
		string? path = node.Tag as string;
		if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;
		var verified_filters = clear_filtered_filesystem_node_childs(node, filters);
		try {
			// Add directories
			var existing = node.Nodes
				.Cast<TreeNode>()
				.Select(n => n.Tag as string)
				.Where(p => p != null)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			foreach (var dir in get_directories(path)) {
				if (existing.Contains(dir)) continue;
				try {
					string dirName = Path.GetFileName(dir);
					var dirNode = new TreeNode(dirName) { Tag = dir };
					try {
						if (get_directories(dir).Length > 0 || get_files(dir).Length > 0)
							dirNode.Nodes.Add(dummy_node);
					} catch { }
					node.Nodes.Add(dirNode);
				} catch { }
			}
			// Add files - filtered
			foreach (var file in get_files(path)) {
				if (existing.Contains(file)) continue;
				try {
					string fileName = Path.GetFileName(file);
					if ( verified_filters.ContainsKey(fileName) ) { 
						if ( !verified_filters[fileName] ) continue; 
					} else {
						if ( !verify_filters(fileName, filters)) continue;
					}
					var fileNode = new TreeNode(fileName) { Tag = file };
					node.Nodes.Add(fileNode);
				} catch { }
			}
		}
		catch (Exception ex) {
			node.Nodes.Add(new TreeNode($"Error: {ex.Message}"));
		}
	}
	private static string[] get_directories(string path) {
		try {
			// Fix paths like "G:" to "G:\"
			if (!string.IsNullOrEmpty(path) && path.Length == 2 && path[1] == ':') {
				path += @"\";
			}
			return Directory.GetDirectories(path);
		}
		catch {
			// Skip unreadable/inaccessible directories
			return Array.Empty<string>();
		}
	}
	private static string[] get_files(string path) {
		try {
			// Fix paths like "G:" to "G:\"
			if (!string.IsNullOrEmpty(path) && path.Length == 2 && path[1] == ':') {
				path += @"\";
			}
			return Directory.GetFiles(path);
		}
		catch {
			return Array.Empty<string>();
		}
	}
	public static List<string> get_fullpath(List<TreeNode> list) {
		var result = new List<string>();
		foreach (var node in list)
		{
			string path = node.FullPath;
			// Optional: If your tree stores custom paths in .Tag
			if (node.Tag is string tagPath) path = tagPath;
			if (File.Exists(path) || Directory.Exists(path)) result.Add(path);
		}
		return result;
	}
	public static TreeNode? get_pointed_node(TreeView tree) {
		if (tree == null) return null;
		// Convert the current screen mouse position to tree-relative coordinates
		Point localPos = tree.PointToClient(Control.MousePosition); 
		// Get the node at that point
		return tree.GetNodeAt(localPos);
	}
	public static void collapse(TreeView tree, int level) {
        if (tree == null) return;

        // Begin update to prevent flickering
        tree.BeginUpdate();

        // Helper method to collapse nodes recursively
        void CollapseNodes(TreeNodeCollection nodes, int currentLevel)
        {
            foreach (TreeNode node in nodes)
            {
                // Collapse the node if its level is greater than or equal to the specified level
                if (currentLevel >= level)
                {
                    node.Collapse();
                }
                // Recursively process child nodes
                CollapseNodes(node.Nodes, currentLevel + 1);
            }
        }

        // Start collapsing from the top-level nodes (level 0)
        CollapseNodes(tree.Nodes, 0);

        // End update to refresh the TreeView
        tree.EndUpdate();
    }
	public static TreeNode? get_parent_node(TreeNode? node) {
		if (node == null) return null;
		return node.Parent;
	}
	public static bool is_node_expanded(TreeNode? node) {
		if (node == null)
			return false;

		return node.IsExpanded;
	}
	
	private static string? get_path_from_selected_node(DarkTreeView explorer){
		TreeNode node = explorer.SelectedNode; 
		if (node == null) return null;
		if (node.Tag == null) return null;
		if (node.Tag is string path){
			return path;
		} else {
			return null;
		}
	}
    public static DarkTreeView new_file_explorer_dark_tree() {
		return new_file_explorer_dark_tree(new List<string>());
	}
    public static TreeNode? get_selected_one_node(DarkTreeView tree) {
        var nodes = tree.GetSelectedNodes();
        if (nodes.Count != 1) return null;        
        return nodes[0];
    }
	public static DarkTreeView new_file_explorer_dark_tree(List<string> directories) {
		DarkTreeView explorer = new_dark_tree("Explorer");
		foreach( string path in get_drives() ){
			join( explorer, new_multiselection_tree(path) );
		}
		join(explorer, new_dummy_tree("*"));
		foreach( string path in directories ){
			if (! is_dir(path)) continue ;
			join( explorer, new_multiselection_tree(path) );
		}
		set_as_filesystem_tree(explorer);
        var context_menu = add_context_menu(explorer, new List<object>{ 
            "Open File/Directory",
            "Open CMD",
            "Open CMD (admin)"
		});
        set_action(context_menu, "Open CMD", (s,e) => {
            ToolStripMenuItem clickedItem = s as ToolStripMenuItem;
            if (clickedItem == null) return ;
            ContextMenuStrip ownerMenu = clickedItem.Owner as ContextMenuStrip;
            if (ownerMenu == null) return ;
            DarkTreeView exp = ownerMenu.SourceControl as DarkTreeView;
            if (exp == null) return ;
			string path = get_path_from_selected_node(exp);
			if (string.IsNullOrEmpty(path)) return ;
			if ( !is_dir(path) && !is_file(path) ) return ;
			open_in_cmd(path);
		});
        set_action(context_menu, "Open CMD (admin)", (s,e)=>{ // TESTING
            ToolStripMenuItem clickedItem = s as ToolStripMenuItem;
            if (clickedItem == null) return ;
            ContextMenuStrip ownerMenu = clickedItem.Owner as ContextMenuStrip;
            if (ownerMenu == null) return ;
            DarkTreeView exp = ownerMenu.SourceControl as DarkTreeView;
            if (exp == null) return ;
			string path = get_path_from_selected_node(exp);
			if (string.IsNullOrEmpty(path)) return ;
			if ( !is_dir(path) && !is_file(path) ) return ;
			open_in_cmd_adm(path);
        });
        explorer.MouseClick += (s,e) => {
            var node = get_selected_one_node(explorer);
            if (node != null && node.Tag is string path) {
                if ( is_dir(path) ) {
                    filter_filesystem_node(node, new List<string>() );
                } 
            }
		};
        explorer.KeyDown += (s,e) => {
            if (e.KeyCode == Keys.Right) {
				var node = get_selected_one_node(explorer);
                if (node != null && node.Tag is string path) {
                    if ( is_dir(path) ) {
                        filter_filesystem_node(node, new List<string>() );
                    } 
                }
                return ;
			}
        };
        return explorer;
	}
}

public static class Incantation_DIALOG {
	public static string? open_dialog(string extension) {
        using (var openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = $"{extension} files (*.{extension})|*.{extension}|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
        }
        return null;
    }
	public static string? open_dialog(string extension, string starting_dir) {
		using (var openFileDialog = new OpenFileDialog())
		{
			openFileDialog.Filter = $"{extension} files (*.{extension})|*.{extension}|All files (*.*)|*.*";
			openFileDialog.RestoreDirectory = true;

			if (!string.IsNullOrEmpty(starting_dir) && Directory.Exists(starting_dir))
			{
				openFileDialog.InitialDirectory = starting_dir;
			}

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				return openFileDialog.FileName;
			}
		}
		return null;
	}
	public static string? save_dialog(string extension)	{
		using (var saveFileDialog = new SaveFileDialog())
		{
			saveFileDialog.Filter = $"{extension} files (*.{extension})|*.{extension}|All files (*.*)|*.*";
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.DefaultExt = extension;

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				return saveFileDialog.FileName;
			}
		}
		return null;
	}
	public static string? save_dialog(string extension, string starting_dir) {
		using (var saveFileDialog = new SaveFileDialog())
		{
			saveFileDialog.Filter = $"{extension} files (*.{extension})|*.{extension}|All files (*.*)|*.*";
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.DefaultExt = extension;

			if (!string.IsNullOrEmpty(starting_dir) && Directory.Exists(starting_dir))
			{
				saveFileDialog.InitialDirectory = starting_dir;
			}

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				return saveFileDialog.FileName;
			}
		}
		return null;
	}
	public static bool move_to_dialog(List<string> fullpaths, string destination) {
        if (fullpaths.Count ==0 ) return false;
		if (!is_dir(destination)) {
			MessageBox.Show("Please select a valid destination folder.");
			return false; 
		}
		if (!confirmation_dialog("Confirmation","Are You Sure to Move/Overwrite these files? This operation will overwrite same filenames, use windows file explorer for full control.")) return false;
		LaunchExplorerCopy(fullpaths, destination, move: true);
		return true;
    }
	public static bool copy_to_dialog(List<string> fullpaths, string destination) {
		if (fullpaths.Count ==0 ) return false;
		if (!is_dir(destination)) {
			MessageBox.Show("Please select a valid destination folder.");
			return false; 
		}
		if (!confirmation_dialog("Confirmation","Are You Sure to Copy/Overwrite these files? This operation will overwrite same filenames, use windows file explorer for full control.")) return false;
        LaunchExplorerCopy(fullpaths, destination, move: false);
		return true;
    }
	public static bool delete_dialog(List<string> fullpaths){
		if (fullpaths.Count ==0 ) return false;
		if (!confirmation_dialog("Confirmation","Are You Sure to Delete These Files?")) return false;
		MessageBox.Show("If the media supports the Files Will be Moved to Trash Bin");
		LaunchExplorerMoveToBinOrDelete(fullpaths);
		return true;
	}
	private static void LaunchExplorerCopy_OLD(List<string> fullpaths, string destination, bool move) {
        if (fullpaths == null || fullpaths.Count == 0)
            throw new ArgumentException("File list cannot be null or empty.", nameof(fullpaths));
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Destination path cannot be null or empty.", nameof(destination));
        if (!Directory.Exists(destination))
            throw new ArgumentException($"Destination folder does not exist: {destination}", nameof(destination));

        string tempScript = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ps1");
        string copyMethod = move ? "MoveHere" : "CopyHere";

        try
        {
            // Escape special characters for PowerShell (double quotes and backticks)
            var psFilePaths = fullpaths.Select(f => f.Replace("\"", "`\"").Replace("`", "``"));

            // Build PowerShell array of file paths: @("C:\path\file1", "C:\path\file2", ...)
            string filesArray = "@(" + string.Join(", ", psFilePaths.Select(f => $"\"{f}\"")) + ")";

            // Escape destination path
            string escapedDestination = destination.Replace("\"", "`\"").Replace("`", "``");

            string psScript = $@"
$ErrorActionPreference = 'Stop'
$shell = New-Object -ComObject Shell.Application
$dest = $shell.NameSpace(""{escapedDestination}"")
if ($null -eq $dest) {{
    Write-Error ""Destination folder not found: {escapedDestination}""
    exit 1
}}
$files = {filesArray}
foreach ($file in $files) {{
    $folderPath = Split-Path $file
    $fileName = Split-Path $file -Leaf
    $folder = $shell.NameSpace($folderPath)
    if ($null -eq $folder) {{
        Write-Warning ""Folder not found: $folderPath""
        continue
    }}
    $item = $folder.ParseName($fileName)
    if ($null -eq $item) {{
        Write-Warning ""File not found: $fileName in $folderPath""
        continue
    }}
    $dest.{copyMethod}($item, 16) # 16 = Suppress overwrite prompt
}}
";

            File.WriteAllText(tempScript, psScript, new UTF8Encoding(true));

            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -NoProfile -File \"{tempScript}\"",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true  
                }
            })
            {
                process.Start();
                string errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"PowerShell script failed: {errorOutput}");
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to execute file {copyMethod.ToLower()} operation: {ex.Message}", ex);
        }
        finally
        {
            // Clean up temporary script
            if (File.Exists(tempScript))
            {
                try
                {
                    File.Delete(tempScript);
                }
                catch (Exception ex)
                {
                    // Log cleanup failure if needed, but don't throw
                    Console.WriteLine($"Failed to delete temporary script {tempScript}: {ex.Message}");
                }
            }
        }
    }
	private static void LaunchExplorerCopy(List<string> fullpaths, string destination, bool move) {
		if (fullpaths == null || fullpaths.Count == 0)
			throw new ArgumentException("File list cannot be null or empty.", nameof(fullpaths));

		if (!Directory.Exists(destination))
			throw new ArgumentException("Destination folder does not exist.", nameof(destination));

		Type shellType = Type.GetTypeFromProgID("Shell.Application");
		dynamic shell = Activator.CreateInstance(shellType);
		dynamic destFolder = shell.NameSpace(destination);

		foreach (var path in fullpaths)
		{
			string folderPath = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path);

			dynamic folder = shell.NameSpace(folderPath);
			dynamic item = folder?.ParseName(fileName);

			if (item != null)
			{
				if (move)
					destFolder.MoveHere(item, 16);
				else
					destFolder.CopyHere(item, 16);
			}
		}
	}
	private static void LaunchExplorerMoveToBinOrDelete_OLD(List<string> fullpaths)	{
		if (fullpaths == null || fullpaths.Count == 0) throw new ArgumentException("File list cannot be null or empty.", nameof(fullpaths));
		string tempScript = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ps1");
		try {
			// Escape special characters
			var psFilePaths = fullpaths.Select(f => f.Replace("\"", "`\"").Replace("`", "``"));
			string filesArray = "@(" + string.Join(", ", psFilePaths.Select(f => $"\"{f}\"")) + ")";
			// powershell script 
			string psScript = $@"
				$ErrorActionPreference = 'Stop'
				Add-Type -AssemblyName Microsoft.VisualBasic
				$shell = New-Object -ComObject Shell.Application
				$files = {filesArray}

				foreach ($file in $files) {{
					if (-not (Test-Path $file)) {{
						Write-Warning ""File not found: $file""
						continue
					}}

					# Attempt to delete to Recycle Bin using .NET (with UI and fallback)
					try {{
						if ((Get-Item $file).PSIsContainer) {{
							[Microsoft.VisualBasic.FileIO.FileSystem]::DeleteDirectory(
								$file,
								'OnlyErrorDialogs', 
								'SendToRecycleBin'  
							)
						}} else {{
							[Microsoft.VisualBasic.FileIO.FileSystem]::DeleteFile(
								$file,
								'OnlyErrorDialogs', 
								'SendToRecycleBin'  
							)
						}}
					}} catch {{
						Write-Warning ""Delete failed: $file. $_""
					}}
				}}
			";

			File.WriteAllText(tempScript, psScript);

			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "powershell.exe",
					Arguments = $"-ExecutionPolicy Bypass -NoProfile -File \"{tempScript}\"",
					UseShellExecute = false,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			})
			{
				process.Start();
				string errorOutput = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					throw new InvalidOperationException($"PowerShell script failed: {errorOutput}");
				}
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to send files to Recycle Bin or delete: {ex.Message}", ex);
		}
		finally
		{
			if (File.Exists(tempScript))
			{
				try { File.Delete(tempScript); }
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to delete temporary script {tempScript}: {ex.Message}");
				}
			}
		}
	}
	private static void LaunchExplorerMoveToBinOrDelete(List<string> fullpaths) {
		if (fullpaths == null || fullpaths.Count == 0)
			throw new ArgumentException("File list cannot be null or empty.", nameof(fullpaths));

		foreach (var path in fullpaths)
		{
			if (!File.Exists(path) && !Directory.Exists(path))
			{
				Console.WriteLine($"File not found: {path}");
				continue;
			}

			try
			{
				if (Directory.Exists(path))
				{
					FileSystem.DeleteDirectory(
						path,
						UIOption.OnlyErrorDialogs,
						RecycleOption.SendToRecycleBin
					);
				}
				else
				{
					FileSystem.DeleteFile(
						path,
						UIOption.OnlyErrorDialogs,
						RecycleOption.SendToRecycleBin
					);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Delete failed: {path}. {ex.Message}");
			}
		}
	}
	public static bool confirmation_dialog(string title, string message) {
		DialogResult result = MessageBox.Show(
			message,
			title,
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question,
			MessageBoxDefaultButton.Button2
		);

		return result == DialogResult.Yes;
	}
	// >>>
	public static string? input_dialog(Form owner,string title, string message, string default_text) {
		using (Form form = new Form())
		using (Label label = new Label())
		using (TextBox textBox = new TextBox())
		using (Button buttonOk = new Button())
		using (Button buttonCancel = new Button()) {
			form.Text = title;
			form.StartPosition = FormStartPosition.CenterParent;
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.ShowInTaskbar = false;
			form.ClientSize = new Size(400, 140);

			label.Text = message;
			label.AutoSize = false;
			label.SetBounds(10, 10, 380, 30);

			textBox.SetBounds(10, 45, 380, 23);
			textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			textBox.Text = default_text;

			buttonOk.Text = "OK";
			buttonOk.DialogResult = DialogResult.OK;
			buttonOk.SetBounds(220, 80, 80, 30);

			buttonCancel.Text = "Cancel";
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.SetBounds(310, 80, 80, 30);

			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;
			
			form.Shown += (s, e) => {
				textBox.Focus();
				textBox.SelectAll();
			};
			
			var result = form.ShowDialog(owner);

			return result == DialogResult.OK ? textBox.Text : null;
		}
	}
	// <<< 
}

// custom widget classes 
public class DarkTabControl : TabControl {
    // == attributes 
    private Color textColor = Color.FromArgb(220, 220, 220); // Matches Notepad_Form text color
    private Color tabBackColor_selected = Color.FromArgb(10, 10, 15);
    private Color tabBackColor_notselected = Color.FromArgb(0, 0, 0);
    private Color borderColor = Color.FromArgb(0, 0, 255); // Thin border color
    // == methods 
    public void change_border_color(Color color) {
        this.borderColor = color;
    }
    // -- overrides
	protected override void OnDrawItem(DrawItemEventArgs e) {
		TabPage tab = this.TabPages[e.Index];
		bool selected = (e.Index == this.SelectedIndex);
        Color tabBackColor = selected ? tabBackColor_selected : tabBackColor_notselected;
		// Draw tab background
		using (SolidBrush brush = new SolidBrush(tabBackColor)) {
			e.Graphics.FillRectangle(brush, e.Bounds);
		}
		// Draw tab text
		TextRenderer.DrawText(
			e.Graphics,
			tab.Text,
			this.Font,
			e.Bounds,
			textColor,
			TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
		);
		// Draw a thin border around the selected tab
		if (selected){
			using (Pen pen = new Pen(borderColor, 1)) {
				Rectangle borderRect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
				e.Graphics.DrawRectangle(pen, borderRect);
			}
		}
	}
	protected override void OnPaint(PaintEventArgs e) {
		// Clear the entire control with the background color
		using ( SolidBrush brush = new SolidBrush(Color.FromArgb(10, 10, 15)) ) {
			e.Graphics.FillRectangle(brush, this.ClientRectangle);
		}
		// Draw the content area (below tabs) with the same background
		Rectangle contentRect = new Rectangle(0, this.ItemSize.Height, this.Width, this.Height - this.ItemSize.Height);
		using (SolidBrush brush = new SolidBrush(Color.FromArgb(10, 10, 15))) {
			e.Graphics.FillRectangle(brush, contentRect);
		}
		// Draw a thin border around the content area
		using (Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1)) {
			Rectangle borderRect = new Rectangle(0, this.ItemSize.Height, this.Width - 1, this.Height - this.ItemSize.Height - 1);
			e.Graphics.DrawRectangle(pen, borderRect);
		}
		// Draw each tab
		for (int i = 0; i < this.TabCount; i++) {
			Rectangle tabRect = this.GetTabRect(i);
			DrawItemEventArgs args = new DrawItemEventArgs(
				e.Graphics,
				this.Font,
				tabRect,
				i,
				this.SelectedIndex == i ? DrawItemState.Selected : DrawItemState.Default
			);
			OnDrawItem(args);
		}
	}
	protected override void OnPaintBackground(PaintEventArgs pevent) {
		// Do nothing to prevent default background painting (avoids white parts)
	}
    // -- constructors
    public DarkTabControl() {
		// Enable custom drawing and optimize painting
		this.SetStyle(ControlStyles.UserPaint, true);
		this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.DrawMode = TabDrawMode.OwnerDrawFixed;
		this.ItemSize = new Size(100, 24); // Fixed tab size
	}
}

// modified for notepad-- 
public class DarkTreeView : MultiSelectTreeView {
	public DarkTreeView() {
		// Use owner-drawn mode for full custom rendering
		this.DrawMode = TreeViewDrawMode.OwnerDrawText;
		this.BackColor = Color.FromArgb(10, 10, 15);
		this.ForeColor = Color.FromArgb(255, 255, 255);
		this.BorderStyle = BorderStyle.None;
		this.HideSelection = false;
		this.FullRowSelect = true;

		this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		this.SetStyle(ControlStyles.ResizeRedraw, true);
	}
	public override void ClearSelectedNodes() {
        foreach (var node in selectedNodes) {
			// InvalidateNode(node);
            node.BackColor = this.BackColor;
            node.ForeColor = DetermineNodeColor(node);
        }
        selectedNodes.Clear();
    }
	protected override void OnDrawNode(DrawTreeNodeEventArgs e)	{
		TreeNode node = e.Node;
		bool isSelected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
		//bool isSelected = SelectedNodes.Contains(e.Node);
		bool isFocused = (e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused;

		// Respect node-assigned colors first
		Color backColor = node.BackColor.IsEmpty ? this.BackColor : node.BackColor;
		Color textColor = node.ForeColor.IsEmpty ? DetermineNodeColor(node) : node.ForeColor;

		// Background
		using (SolidBrush bgBrush = new SolidBrush(backColor))
			e.Graphics.FillRectangle(bgBrush, e.Bounds);

		// Text
		TextRenderer.DrawText(
			e.Graphics,
			node.Text,
			this.Font,
			e.Bounds,
			textColor,
			TextFormatFlags.VerticalCenter | TextFormatFlags.Left
		);

		// Optional: border around focused node
		if (isSelected && isFocused)
		{
			using (Pen pen = new Pen(Color.FromArgb(0, 0, 255)))
			{
				var rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
				e.Graphics.DrawRectangle(pen, rect);
			}
		}
	}
	protected override void RestoreNodeColor(TreeNode node) {
		node.BackColor = this.BackColor;
		node.ForeColor = DetermineNodeColor(node);
	}
	private Color DetermineNodeColor(TreeNode node) {
		var media_color = Color.Red;
		var exe_color = Color.Yellow;
		var special_file_color = Color.Cyan;
		var editable_color = Color.Magenta;
		if (node.Tag is string path) {
			if (Directory.Exists(path))
				return Color.FromArgb(0,255,0); // Folder
			if (File.Exists(path)) {
                if ( is_code_file(path) ) return exe_color; 
//				string ext = get_extension(path);
//                
//				switch (ext) { 
//					case ".exe": return special_file_color;
//					case ".ini": return special_file_color; 
//					case ".json": return special_file_color;
//					case ".bat": return special_file_color;
//					case ".dll": return special_file_color; 
//					case ".txt": return editable_color; 
//					case ".pdf": return media_color; 
//					case ".mp3": return media_color;
//					case ".mp4": return media_color;
//					case ".mkv": return media_color;
//					case ".zip": return special_file_color;
//					case ".7z": return special_file_color;
//				}
				return Color.FromArgb(255,255,255);     // File
			} 
		}
		// Default
		return this.ForeColor;
	}
	protected override void OnPaintBackground(PaintEventArgs pevent) {
		// Paint custom background to prevent default flicker
		using (SolidBrush bg = new SolidBrush(this.BackColor))
		{
			pevent.Graphics.FillRectangle(bg, this.ClientRectangle);
		}
	}
}

public class MultiSelectTreeView : TreeView {
	protected bool suppressSelection = false;
    protected readonly List<TreeNode> selectedNodes = new List<TreeNode>();
    protected TreeNode? lastSelectedNode = null;
	protected Color selected_back_color = Color.Blue;

    public IReadOnlyList<TreeNode> SelectedNodes => selectedNodes.AsReadOnly();

    public MultiSelectTreeView() {
        this.HideSelection = false;
    }

    protected override void OnBeforeSelect(TreeViewCancelEventArgs e) {
        if (suppressSelection) e.Cancel = true;
        base.OnBeforeSelect(e);
    }

    protected override void OnMouseDown(MouseEventArgs e) {
		if (e.Button != MouseButtons.Left) {
			base.OnMouseDown(e);
			return;
		}		
		suppressSelection = true;
        TreeNode? clickedNode = this.GetNodeAt(e.Location);
        if (clickedNode == null) return;

        if (ModifierKeys.HasFlag(Keys.Shift) && lastSelectedNode != null) {
            // SHIFT: Select range
            var range = GetNodeRange(lastSelectedNode, clickedNode);
            ClearSelectedNodes();
            foreach (var node in range) {
                AddNodeToSelection(node);
            }
        }
        else if (ModifierKeys.HasFlag(Keys.Control)) {
            // CTRL: Toggle selection
            if (selectedNodes.Contains(clickedNode)) {
                this.RemoveNodeFromSelection(clickedNode);
            } else {
                AddNodeToSelection(clickedNode);
                lastSelectedNode = clickedNode;
            }
        }
        else {
            // No modifier: Single selection
            ClearSelectedNodes();
            AddNodeToSelection(clickedNode);
            lastSelectedNode = clickedNode;
        }

        base.OnMouseDown(e);
		suppressSelection = false;
    }

    public virtual void ClearSelectedNodes() {
        foreach (var node in selectedNodes) {
			// InvalidateNode(node);
            node.BackColor = this.BackColor;
            node.ForeColor = this.ForeColor;
        }
        selectedNodes.Clear();
    }

    private void AddNodeToSelection(TreeNode node) {
        if (!selectedNodes.Contains(node)) {
            selectedNodes.Add(node);
            node.BackColor = this.selected_back_color; 
            node.ForeColor = SystemColors.HighlightText;
        }
    }

    protected virtual void RestoreNodeColor(TreeNode node) {
		node.BackColor = this.BackColor;
		node.ForeColor = this.ForeColor; 
	}

	protected virtual void RemoveNodeFromSelection(TreeNode node) {
		if (selectedNodes.Remove(node)) {
			this.RestoreNodeColor(node);
		}
	}

    private List<TreeNode> GetNodeRange(TreeNode start, TreeNode end) {
        List<TreeNode> allNodes = GetAllNodes(visibleOnly: true);
        int iStart = allNodes.IndexOf(start);
        int iEnd = allNodes.IndexOf(end);

        if (iStart < 0 || iEnd < 0) return new List<TreeNode>();

        if (iStart > iEnd) {
            var tmp = iStart;
            iStart = iEnd;
            iEnd = tmp;
        }

        return allNodes.GetRange(iStart, iEnd - iStart + 1);
    }

    private List<TreeNode> GetAllNodes(bool visibleOnly) {
        List<TreeNode> list = new List<TreeNode>();
        foreach (TreeNode root in this.Nodes) {
            CollectNodes(root, list, visibleOnly);
        }
        return list;
    }

    private void CollectNodes(TreeNode node, List<TreeNode> list, bool visibleOnly) {
        list.Add(node);
        if (node.IsExpanded || !visibleOnly) {
            foreach (TreeNode child in node.Nodes) {
                CollectNodes(child, list, visibleOnly);
            }
        }
    }
	
	protected override void OnAfterSelect(TreeViewEventArgs e) {
		// Do NOT modify custom selection on keyboard input
		// Let TreeView.SelectedNode update naturally
		base.OnAfterSelect(e);
	}
	
	protected override void OnKeyDown(KeyEventArgs e) {
		bool isArrowKey = e.KeyCode == Keys.Up || e.KeyCode == Keys.Down;

		if (!isArrowKey) {
			base.OnKeyDown(e);
			return;
		}

		TreeNode? anchor = lastSelectedNode ?? this.SelectedNode;

		// Let TreeView process the key normally first
		base.OnKeyDown(e);

		// Delay custom logic to run *after* TreeView updates SelectedNode
		this.BeginInvoke((MethodInvoker)delegate {
			TreeNode? focused = this.SelectedNode;
			if (focused == null) return;

			if (e.Shift && anchor != null) {
				// SHIFT: range from anchor to new focus
				var range = GetNodeRange(anchor, focused);
				ClearSelectedNodes();
				foreach (var node in range)
					AddNodeToSelection(node);
			}
			else if (!e.Control && !e.Shift) {
				// No modifiers: reset selection to focused node
				ClearSelectedNodes();
				AddNodeToSelection(focused);
				lastSelectedNode = focused;
			}
			else if (!e.Shift) {
				// CTRL only: update anchor but don't select
				lastSelectedNode = focused;
			}
		});
	}

	public List<TreeNode> GetSelectedNodes() {
		return new List<TreeNode>(selectedNodes);
	}
}

public class OverlayForm : Form {
    protected Label LabelRef;
    protected Form? parent_form;
    private Timer? fadeTimer;
    private Color background_color = Color.FromArgb(30,30,30);
    private double max_opacity = 0.3;
    private double opacity_step = 0.01;
    // == methods 
    public OverlayForm(Form parent) : this(parent, "") {}
    public OverlayForm(Form parent, string message) {
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.BackColor = this.background_color;
        this.Opacity = 0;
        this.Bounds = parent.Bounds;    
        this.LabelRef = new Label {
            Text = message,
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Font = new Font("Consolas", 14),
            Location = new Point(this.Width / 2 - 100, this.Height / 2),
//            Dock = DockStyle.Fill
        };
        this.Controls.Add(this.LabelRef);
        this.parent_form = parent;
        this.Activated += (s,e)=>{
            this.parent_form?.Activate();
        };
        this.Shown += (s,e)=>{
            this.parent_form?.Activate();
        };
    }
    public void update_size() {
        if (this.parent_form == null) return;
        this.Width = this.parent_form.Width;
        this.Height = this.parent_form.Height;
    }
    public void update_position() {
        if (this.parent_form == null) return;
        this.Location = this.parent_form.Location; 
        this.LabelRef.TextAlign = ContentAlignment.MiddleCenter;
        this.LabelRef.Location = new Point(this.Width / 2 - 100, this.Height / 2);
    }
    public void Display(string message, int time = 30) {
        if (string.IsNullOrWhiteSpace(message)) return;
        update_size();
        update_position();
        LabelRef.Text = message;
        if (!Visible) {
            Opacity = 0;
            Show();
        }
        StartFadeAnimation(time);
    }
    private void StartFadeAnimation(int time) {
        fadeTimer?.Stop();
        fadeTimer = new Timer { Interval = time/3 };
        fadeTimer.Tick += (s, e) => {
            if (Opacity < this.max_opacity)
                Opacity += this.opacity_step;
            else {
                fadeTimer.Stop();
                FadeOut(time);
            }
        };
        fadeTimer.Start();
    }
    private void FadeOut(int time) {
        Timer t = new Timer { Interval = time };
        t.Tick += (s, e) => {
            if (Opacity > 0)
                Opacity -= this.opacity_step;
            else {
                t.Stop();
            }
        };
        t.Start();
    }
    // -- overrides 
    protected override CreateParams CreateParams {
        get {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
            return cp;
        }
    }

}

// ===================================== conjuration 
// ... system integration  
public static class Conjuration {
    public static bool start_process(string processname, string arguments) {
        if (string.IsNullOrWhiteSpace(processname)) return false;
        if (!File.Exists(processname)) return false; 
        if (Directory.Exists(processname)) return false;
        Process.Start(processname, arguments);
        return true;
    }
    public static bool start_process_adm(string processname, string arguments) {
        if (string.IsNullOrWhiteSpace(processname)) return false;
        if (!File.Exists(processname)) return false;
        if (Directory.Exists(processname)) return false;
        var psi = new ProcessStartInfo {
            FileName = processname,
            Arguments = arguments,
            Verb = "runas",          // <-- This requests elevation
            UseShellExecute = true   // <-- Required for "runas"
        };
        try {
            Process.Start(psi);
            return true;
        }
        catch (System.ComponentModel.Win32Exception ex) {
            // User clicked "No" on the UAC prompt
            if (ex.NativeErrorCode == 1223) return false;
            throw;
        }
    }
	public static bool default_program_start(string filename) {
		if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
			return false;

		try {
			ProcessStartInfo psi = new ProcessStartInfo {
				FileName = filename,
				UseShellExecute = true // Required to launch with default program
			};
			Process.Start(psi);
			return true;
		}
		catch {
			return false; // Handle errors like no association, permissions, etc.
		}
	}
	public static bool open_in_windows_explorer(string filename) {
		if (string.IsNullOrWhiteSpace(filename))
			return false;

		try {
			if (Directory.Exists(filename)) {
				// It's a folder: open it directly
				Process.Start("explorer.exe", $"\"{filename}\"");
			}
			else if (File.Exists(filename)) {
				// It's a file: select it in Explorer
				Process.Start("explorer.exe", $"/select,\"{filename}\"");
			}
			else {
				return false; // File or folder does not exist
			}
			return true;
		}
		catch {
			return false;
		}
	}
	public static bool open_in_cmd(string filename) {
		if (string.IsNullOrWhiteSpace(filename)) return false;
		try {
			string? directory = null;
			if (Directory.Exists(filename)) {
				directory = filename;
			}
			else if (File.Exists(filename)) {
				directory = Path.GetDirectoryName(filename);
			}
			if (directory != null) {
				Process.Start(new ProcessStartInfo {
					FileName = "cmd.exe",
					Arguments = $"/K cd /d \"{directory}\"",
					UseShellExecute = true
				});
				return true;
			}
			return false; // File or folder doesn't exist
		}
		catch {
			return false;
		}
	}
    public static bool open_in_cmd_adm(string filename) { // TESTING
		if (string.IsNullOrWhiteSpace(filename)) return false;
		try {
			string? directory = null;
			if (Directory.Exists(filename)) {
				directory = filename;
			}
			else if (File.Exists(filename)) {
				directory = Path.GetDirectoryName(filename);
			}
			if (directory != null) {
				Process.Start(new ProcessStartInfo {
					FileName = "cmd.exe",
					Arguments = $"/K cd /d \"{directory}\"",
                    Verb = "runas",
					UseShellExecute = true
				});
				return true;
			}
			return false; // File or folder doesn't exist
		}
		catch {
			return false;
		}
	}
	public static bool default_program_edit(string filename) {
		if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename)) return false;
		try {
			ProcessStartInfo psi = new ProcessStartInfo {
				FileName = filename,
				Verb = "edit",
				UseShellExecute = true // Required to launch with default program
			};
			Process.Start(psi);
			// return true;
			// var psi = new ProcessStartInfo(filename) {
				// UseShellExecute = true
			// };
			// if (!psi.Verbs.Contains("edit", StringComparer.OrdinalIgnoreCase)) return false;
			// psi.Verb = "edit";
			// Process.Start(psi);
			return true;
		}
		catch (Exception ex) {
			MessageBox.Show(
				$"Failed to open file for editing.\n\nFile:\n{filename}\n\nError:\n{ex.Message}",
				"Edit Error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
			return false;
		}

			
	}
	public static bool rename_file(string filename, string new_name) {
		if (string.IsNullOrWhiteSpace(filename)) return false;
		if (string.IsNullOrWhiteSpace(new_name)) return false;
		try {
			string fullPath = Path.GetFullPath(filename);
			if (!File.Exists(fullPath)) return false; 
			string extension = Path.GetExtension(fullPath);
			if ( !Path.HasExtension(new_name) ) new_name += extension;
			if (new_name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return false;
			string directory = Path.GetDirectoryName(fullPath)!;
			string newFullPath = Path.Combine(directory, new_name);
			if (File.Exists(newFullPath)) return false;
			File.Move(fullPath, newFullPath);
			return true;
		} catch (UnauthorizedAccessException) {
			return false;
		} catch (IOException) {
			return false;
		} catch {
			return false;
		}
	}
    
    [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT lpPoint);
    struct POINT { public int X; public int Y; }
    private const uint GW_HWNDNEXT = 2;
    private const uint WM_MOUSEWHEEL = 0x020A;
    // WHEEL_DELTA is 120 units per notch
    private const int WHEEL_DELTA = 120;
    public static void MouseScrollBackWindow(Form currentForm, bool up) {
        IntPtr below = GetWindow(currentForm.Handle, GW_HWNDNEXT);
        if (below == IntPtr.Zero) return;
        GetCursorPos(out POINT pt);
        int lParam = (pt.X & 0xFFFF) | ((pt.Y & 0xFFFF) << 16);
        int delta = up ? WHEEL_DELTA : -WHEEL_DELTA;
        int wParam = (delta << 16); // no key state
        PostMessage(below, WM_MOUSEWHEEL, (IntPtr)wParam, (IntPtr)lParam);
    }
    // -- firewall 
    /*
    public static void BlockApplication(string ruleName, string appPath) {
        // Get firewall policy
        INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
            Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        // Create a new rule
        INetFwRule rule = (INetFwRule)Activator.CreateInstance(
            Type.GetTypeFromProgID("HNetCfg.FWRule"));
        rule.Name = ruleName;
        rule.ApplicationName = appPath;
        rule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
        rule.Enabled = true;
        // Add rule
        firewallPolicy.Rules.Add(rule);
    }
    public static void UnblockApplication(string ruleName) {
        INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
            Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

        // Remove rule by name
        firewallPolicy.Rules.Remove(ruleName);
    }
    */
    /*
    public static bool block_app_windows_firewall(string ruleName, string dirPath) {
        if ( !Directory.Exists(dirPath) ) return false;
        try {
            using (PowerShell ps = PowerShell.Create()) {
                ps.AddScript($@"
                    New-NetFirewallRule -DisplayName '{ruleName}' `
                                        -Direction Outbound `
                                        -Program '{dirPath}\*' `
                                        -Action Block
                ");
                Collection<PSObject> results = ps.Invoke();
                return ps.HadErrors == false;
            }
        } catch {
            return false;
        }
    }
    public static bool unblock_app_windows_firewall(string ruleName) {
        try {
            using (PowerShell ps = PowerShell.Create()) {
                ps.AddScript($@"Remove-NetFirewallRule -DisplayName '{ruleName}'");
                ps.Invoke();
                return ps.HadErrors == false;
            }
        } catch {
            return false;
        }
    }
    */
}

// -- END 