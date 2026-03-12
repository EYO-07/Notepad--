// Notepad-- : Simpler version of Notepad++ 
// -- BEGIN 

namespace Notepad__;
// -- 
using System.IO;
using System.Text;
using System.Text.Json;
using ScintillaNET;
using Codex;
using static Codex.Transmutation;
using static Codex.Incantation;
using static Codex.Incantation_TREEVIEW;
using static Codex.Incantation_DIALOG;
using static Codex.Incantation_SCINTILLA;
using static Codex.Incantation_PANEL; 
using static Codex.Incantation_CONTEXTMENU; 
using static Codex.Incantation_TEXTBOX;
using static Codex.Incantation_TABS;
using static Codex.Incantation_EVENTS;
// -- 
using BorderStyle = System.Windows.Forms.BorderStyle;

public class Form_DATA {
    public int Width { get; set; }
    public int Height { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
	public List<string> Directories { get; set; }
	public List<string> LeftFiles { get; set; }
	public List<string> RightFiles { get; set; }
	public float FontSize { get; set; } 
	public string DialogDir { get;set; }
    public Form_DATA() {
        Width = 1200;
        Height = 600;
        X = 20;
        Y = 20;
		Directories = new List<string>();
		LeftFiles = new List<string>();
		RightFiles = new List<string>();
		DialogDir = get_exec_dir();
		FontSize = 8; 
    }
}

public partial class Notepad__Form : Form {
	// == attributes 
	private Form_DATA? data = null;
	private string data_filename = "Notepad--.sav";
	private SplitContainer main_panel;
	private DarkTabControl left_tabs;
	private DarkTabControl right_tabs;
	private DarkTreeView explorer;
	private Dictionary<string, Scintilla> fullpath_scintilla_map = new();
	private bool hidden_titlebar = true;
	private string unsave_marker = "!! ";
	private float font_step = 0.5f;
	private static Color background_color = Color.FromArgb(10, 10, 15);
	private static Color locked_background_color = Color.FromArgb(0, 15, 0);
	private Scintilla help_scintilla;
    private HashSet<string> autocomplete_hashset = new();
    private string search_token = "";
	// == methods 
	// -- user interface workflow 
	private void _components() {
		this.left_tabs = new_dark_tabs();
		this.right_tabs = new_dark_tabs();
		this.explorer = new_file_explorer_dark_tree(this.data.Directories);
		this.help_scintilla = new_scintilla();
		this.help_scintilla.Text = """
// [ Notepad-- ] : A lesser version of Notepad++, don't like it? Use Notepad++ instead.
// ... build on top of Scintilla5.NET ( https://github.com/desjarlais/Scintilla.NET ) by desjarlais

// Features--
1. "Fixed Dark Theme" // I don't care about your bad taste, it's hardcoded.
2. "Has less language support" // Just use a normal programming language, like a normal person.
3. "Lot of commands using keyboard shortcuts instead of a proper user interface" // Never played a game? Are u a boomer?  
4. "Beeps and Boops Sounds on some keybinds" // Don't know why, it's vibecoded ... 

// Editor Commands
1. Ctrl+N       // New Empty Tab 
2. Ctrl+S       // Save File 
3. Ctrl+H       // Toggle Titlebar ~ Compact Mode 
                // ... use this to access the close button on right corner 
4. Ctrl+R       // Toggle readonly mode changing background color : green safe, blue not safe. 
                // It's updates the autocomplete words 
5. Alt+O        // Toggle Folding/Outlining 
6. Alt+P        // Fold All Markers 
7. Ctrl+Arrows  // Scroll the Editor 
8. Ctrl+Tab     // Change and Focus on Tabs, 
                // ... with tabs focused you can change tabs using arrow keys 
                // ... and shift tab to change between panels.
                // ... tab to return to Editors 

// Tab Commands
1. Right Click on Tab       // Switch the tab between panels
2. Drag Tab                 // Move the window 
3. Ctrl+Right Click on Tab  // Close the tab 

// Explorer Tab Commands 
1. Insert on File Explorer Tab // Add the selected directory as shortcut 
2. Delete on File Explorer Tab // Remove the selected directory from explorer tree 
3. Enter on File Explorer Tab  // Open the File if the language is supported 
""";
	}
	private void _layout() {
		this.StartPosition = FormStartPosition.Manual;
		SBR_set_location_and_position_from_data();
		// tabs
		add_new_tab(this.left_tabs, this.help_scintilla , "?" );
		add_new_tab(this.right_tabs, this.explorer, "File Explorer");
		foreach(var file in this.data.LeftFiles) {
			add_new_scintilla_tab(this.left_tabs, file);
		}
		foreach(var file in this.data.RightFiles) {
			add_new_scintilla_tab(this.right_tabs, file);
		}
		// main panel 
		this.main_panel = new_vertical_split(this.left_tabs, this.right_tabs, 150);
		this.main_panel.Dock = DockStyle.Fill;
		this.Controls.Add(this.main_panel);
	}
	private void _logic() {
		this.FormClosing += (s, e) => {
			SBR_save_session_data();
		};
		this.Activated += (s,e) => {
			this.Opacity = 0.85;
		};
		this.Deactivate += (s,e) => {
			this.Opacity = 0.35;
		};
		key_shortcut(this, "ctrl","h", () => {
			SBR_compact_toggle();
		});
		key_shortcut(this, "ctrl","n", () => {
			add_new_scintilla_tab(this.left_tabs);
		}); 
		// 
		drag_window(this.left_tabs);
		drag_window(this.right_tabs);
		this.left_tabs.MouseClick += tabs_click_handler;
		this.right_tabs.MouseClick += tabs_click_handler;
		//
		this.right_tabs.ShowToolTips = true;
		this.left_tabs.ShowToolTips = true;
		//
		this.explorer.KeyDown += (s,e) => {
			if (e.KeyCode == Keys.Enter) {
				string filename = get_selected_filename_from_explorer(this.explorer);
				if (! add_new_scintilla_tab(this.left_tabs, filename) ) return ;
				this.data.LeftFiles.Add(filename);
			}
			if (e.KeyCode == Keys.Delete) {
				SBR_rem_selected_dir_to_exp();
				return ;
			}
			if (e.KeyCode == Keys.Insert) {
				SBR_add_selected_dir_to_exp();
				return ;
			}
			if (e.KeyCode == Keys.Right) {
				var nodes = this.explorer.GetSelectedNodes();
				if (nodes.Count == 1) {
					var node = nodes[0];
					if (node != null && node.Tag is string path) {
						if ( is_dir(path) ) {
							filter_filesystem_node(node, new List<string>() );
						} 
					}
				}
			}
		}; 
		// some logic features are on add_new_scintilla_tab, scintilla_tab_logic function !
	}
	private void _theme() {
        hide_titlebar(this);
		this.main_panel.BorderStyle = BorderStyle.None;
		// Set colors for the form
		Color background = Color.FromArgb(10, 10, 15);
		Color text = Color.FromArgb(255, 255, 255);
		apply_theme_recursive(this, background, text);
		// 
		// help 
		set_language_folding(this.help_scintilla, "cpp");
		set_cs_style(this.help_scintilla);
		this.help_scintilla.ReadOnly = true;
	} 
	// -- subroutines 
    // explorer tasks 
	private void SBR_add_selected_dir_to_exp() {
		var str_list = get_fullpath( this.explorer.GetSelectedNodes() );
		if (str_list.Count == 0) return ;
		foreach( string path in str_list ){
			if (! is_dir(path)) continue ;
			join( this.explorer, new_multiselection_tree(path) );
			this.data.Directories.Add(path);
		}
	}
	private void SBR_rem_selected_dir_to_exp() {
		TreeNode node = this.explorer.SelectedNode; 
		if (node == null) return ;
		if (node.Tag is string path && Directory.Exists(path)) {
			if (this.data.Directories.Contains(path)){					
				this.data.Directories.Remove(path);
				this.explorer.Nodes.Remove(node);
			}
		}
	}
	// --
    private DarkTabControl? get_focused_tab() {
		if ( this.left_tabs.ContainsFocus ) return this.left_tabs;
		if ( this.right_tabs.ContainsFocus ) return this.right_tabs;
		return null;
	}
	private void SBR_update_font(){
		// not working 
		foreach (TabPage page in this.right_tabs.TabPages){
			var editor = get_first<Scintilla>(page);
			if (editor==null) continue; 
			editor.Styles[Style.Default].Size = (int) Math.Floor( this.data.FontSize );
		}
		foreach (TabPage page in this.left_tabs.TabPages){
			var editor = get_first<Scintilla>(page);
			if (editor==null) continue; 
			editor.Styles[Style.Default].Size = (int) Math.Floor( this.data.FontSize );
		}
	}
	private void SBR_compact_toggle(){
		if (hidden_titlebar) {
			show_titlebar(this);
		} else {
			hide_titlebar(this);
		}
		hidden_titlebar = !hidden_titlebar;
	}
	// --
    private void tabs_click_handler(Object s, MouseEventArgs e) {
		if (e.Button == MouseButtons.Right)	{
			DarkTabControl tabs = (DarkTabControl) s;
			TabPage pointed_page = get_pointed_tab(tabs, e.Location);
			if ((Control.ModifierKeys & Keys.Control) != 0) {
				if ( fullpath_scintilla_map.ContainsKey( pointed_page.ToolTipText ) ) { 
					close_tab(tabs, pointed_page);
					fullpath_scintilla_map.Remove( pointed_page.ToolTipText );
					this.data.LeftFiles.Remove(pointed_page.ToolTipText);
					this.data.RightFiles.Remove(pointed_page.ToolTipText);
				} 
			} else {
				if (pointed_page == null) return;
				switch_view(pointed_page);
			}
		}			
	}
    // --
	private void SBR_set_location_and_position_from_data() {
		if (this.data == null) return ; 
		this.Size = new Size(this.data.Width, this.data.Height);
		this.Location = new Point(this.data.X, this.data.Y);
	}
	private void SBR_save_session_data() {
		if (this.data == null) return ;
		this.data.Width = this.Width;
		this.data.Height = this.Height;
		this.data.X = this.Location.X;
		this.data.Y = this.Location.Y;
		save(
			join(get_exec_dir(), this.data_filename), 
			JsonSerializer.Serialize(this.data)
		);
	}
	// -- 
    private string? get_selected_filename_from_explorer(DarkTreeView explorer) {
		var nodes = this.explorer.GetSelectedNodes();
		if (nodes.Count != 1) return null;
		var node = nodes[0];
		if (node == null) return null;
		if (node.Tag is string path) {
			if ( is_dir(path) || is_file(path) ) return path;
		}
		return null;
	}
	// new tabs 
    private TabPage add_new_tab(DarkTabControl tabs, Control ctrl, string name) {
		TabPage page = add_tab<Panel>(tabs, name);
		Panel panel = get_first<Panel>(page);
		panel.Dock = DockStyle.Fill;
		panel.Controls.Add( ctrl );
		return page;
	}
    private bool add_new_scintilla_tab(DarkTabControl tabs,string filename) {
		if (string.IsNullOrWhiteSpace(filename)) return false;
		if (!is_code_file(filename)) return false;
		if ( this.fullpath_scintilla_map.ContainsKey(filename) ) {	
			// this.fullpath_scintilla_map[filename].Focus();
			return false;
		}
		Scintilla ns = new_scintilla();
		this.fullpath_scintilla_map[filename] = ns;
		load_file(ns, filename);
		TabPage page = add_new_tab(tabs, ns, get_filename(filename) );
		page.ToolTipText = filename;
		// extra logic 
		scintilla_tab_logic(ns, page);
		return true;
	}
	private void add_new_scintilla_tab(DarkTabControl tabs) {
		Scintilla ns = new_scintilla();
		TabPage page = add_new_tab(tabs, ns, "New File" );
		scintilla_tab_logic(ns, page);
	}
	// -- 
    private void get_document_words(Scintilla s) {
        this.autocomplete_hashset.Clear();
        var matches = System.Text.RegularExpressions.Regex.Matches(
            s.Text,
            @"[A-Za-z_]\w+"
        );
        foreach (System.Text.RegularExpressions.Match m in matches) autocomplete_hashset.Add(m.Value);
    }
    private bool is_updating_words = false;
    private CancellationTokenSource wordUpdateCTS;
    public void kill_document_words_async() {
        wordUpdateCTS?.Cancel();
    }
    private async Task get_document_words_async(Scintilla s) {
        if (is_updating_words) return;
        is_updating_words = true;
        wordUpdateCTS?.Cancel();                 // cancel previous job
        wordUpdateCTS = new CancellationTokenSource();
        var token = wordUpdateCTS.Token;
        try {
            string text = s.Text;
            var words = await Task.Run(() => {
                var set = new HashSet<string>();
                var matches = System.Text.RegularExpressions.Regex.Matches(
                    text,
                    @"[A-Za-z_]\w+"
                );
                foreach (System.Text.RegularExpressions.Match m in matches) {
                    if (token.IsCancellationRequested) return set;
                    set.Add(m.Value);
                }
                return set;
            }, token);
            if (!token.IsCancellationRequested) autocomplete_hashset = words;
        }
        finally {
            is_updating_words = false;
        }
    }

    private string get_current_word(Scintilla s) {
        int pos = s.CurrentPosition;
        int start = s.WordStartPosition(pos, true);
        int len = pos - start;
        if (len <= 0) return "";
        return s.GetTextRange(start, len);
    }
    private string build_autocomplete_list(HashSet<string> words, string prefix) {
        var list = new List<string>();
        foreach (var w in words)
            if (w.StartsWith(prefix) && w != prefix)
                list.Add(w);
        list.Sort();
        return string.Join(" ", list);
    }
    private void scintilla_tab_logic(Scintilla ns, TabPage page) {
		ns.TextChanged += (s,e) => {
			page.Text = unsave_marker+ get_filename(page.ToolTipText);
		};
        key_shortcut(ns, "ctrl","f", () => {
            // -- todo 
        });
		key_shortcut(ns, "ctrl","s", () => {
			TabPage current_page = page;
			if (current_page == null) return ;
			string path = current_page.ToolTipText;
			Scintilla editor = ns;
			if (editor==null) return ;
			string content = editor.Text;
			if (is_file(path)){
				save(path, content); 
				this.fullpath_scintilla_map[path] = editor;
				set_fold_and_style(editor, path);
				current_page.Text = get_filename(path);
			} else {
				string _path = save_dialog("txt", this.data.DialogDir);
				if (_path == null) return ;
				current_page.ToolTipText = _path;
				save(_path, content); 
				this.fullpath_scintilla_map[_path] = editor;
				set_fold_and_style(editor, _path);
				current_page.Text = get_filename(_path);
			}
		});	
		key_shortcut(ns, "ctrl","r", async () => {
			toggle_read_only(ns);
            if (ns.ReadOnly) {
                kill_document_words_async();
            } else {
                _ = get_document_words_async(ns); // fire and forget
            }
		});
		ns.CharAdded += (s, e) => {
            var editor = (Scintilla)s;
            if (editor.AutoCActive) return;
            string prefix = get_current_word(editor);
            if (prefix.Length < 3) return;
            string list = build_autocomplete_list(autocomplete_hashset, prefix);
            if (list.Length > 0) editor.AutoCShow(prefix.Length, list);
        };
        toggle_read_only(ns);
	}
	private void toggle_read_only(Scintilla editor){
		editor.ReadOnly = !editor.ReadOnly;
		if (editor.ReadOnly) {
			for (int i = 0; i < 256; i++) {
				editor.Styles[i].BackColor = locked_background_color;
			}
		} else {
			for (int i = 0; i < 256; i++) {
				editor.Styles[i].BackColor = background_color;
			}
			set_fold_and_style(editor, (string) editor.Tag);
		}
	}
	private bool switch_view(TabPage page) {
		if ( is_page_in_tabs(page, this.left_tabs) ) {
			this.right_tabs.TabPages.Add(page);
			string path = page.ToolTipText;
			if ( is_file(path) || is_dir(path) ) {
				this.data.LeftFiles.Remove(path);
				this.data.RightFiles.Add(path);
			}
			return true;
		}
		if ( is_page_in_tabs(page, this.right_tabs) ) {
			this.left_tabs.TabPages.Add(page);
			string path = page.ToolTipText;
			if ( is_file(path) || is_dir(path) ) {
				this.data.RightFiles.Remove(path);
				this.data.LeftFiles.Add(path);
			}
			return true;
		}
		return false;
	}
	// -- constructor 
    public Notepad__Form() {
        InitializeComponent(); 
		this.data = new_object_from_json<Form_DATA>(join(get_exec_dir(), this.data_filename));
		_components(); 
		_layout(); 
		_logic(); 
		_theme(); 
    }
}

























// -- END 