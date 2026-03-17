// Notepad-- : Simpler version of Notepad++ 
// -- BEGIN 
// {Notepad--;Red:BUG,ISSUE;Yellow:?;Cyan:TODO;Silver:SOLVED}

/* === BUG/ISSUE === 
1. Toggle Outlining/Folding : Alt+P don't always put the caret visible - ...
2. Message Overlay : Display OverlayForm steals the window focus and don't return - SOLVED
3. files.c not recognized - SOLVED 
4. custom hightlight not been applied to all lines - SOLVED
5. custom hightlight detecting non-tokens - SOLVED
6. the lexer seems to not be active until text change - SOLVED
*/

/* === TODO ===
1. Tab specialized for searching and replacing content on target tab 
2. is_file_modified(Control ctrl, string filename) 
*/

namespace Notepad__;
// -- 
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ScintillaNET;
// --
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
using static Codex.Conjuration;
//using static Codex.Conjuration_GLOBALHOTKEY;
// -- ambiguities 
using BorderStyle = System.Windows.Forms.BorderStyle;

public class Form_DATA {
    public int Width { get; set; }
    public int Height { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int split_percentage { get; set; }
	public List<string> Directories { get; set; }
	public List<string> LeftFiles { get; set; }
	public List<string> RightFiles { get; set; }
	public float FontSize { get; set; } 
	public string DialogDir { get;set; }
    public Form_DATA() { // defaults 
        Width = 1200;
        Height = 600;
        X = 20;
        Y = 20;
        split_percentage = 55;
		Directories = new List<string>();
		LeftFiles = new List<string>();
		RightFiles = new List<string>();
		DialogDir = get_exec_dir();
		FontSize = 8; 
    }
}

public partial class Notepad__Form : Form {
	// == attributes 
    // -- session data
	private Form_DATA? data = null;
	private string data_filename = "Notepad--.sav";
	// -- gui components
    private SplitContainer main_panel;
	private DarkTabControl left_tabs;
	private DarkTabControl right_tabs;
	private DarkTreeView explorer;
    private Scintilla help_scintilla;
    private OverlayForm message_overlay = null;
    // -- theme
    private static Color background_color = Color.FromArgb(10, 10, 12);
	private static Color locked_background_color = Color.FromArgb(0, 12, 0);
    private static Color border_color = Color.FromArgb(0, 0, 255);
    private static Color locked_border_color = Color.FromArgb(0, 255, 0);
    // -- gui options 
    private double work_opacity = 0.85;
    private double background_opacity = 0.35;   
//    private int split_percentage = 55;
    private bool hidden_titlebar = true;
    // -- 
	private Dictionary<string, Scintilla> fullpath_scintilla_map = new();
    private Dictionary<string, int> fullpath_lines_map = new();
	private string unsave_marker = "!! ";
	private float font_step = 0.5f;
    private HashSet<string> autocomplete_hashset = new();
    private string search_token = "";
    private int char_added_debouncer = 0;
    private int char_added_debouncer_max = 20;
    private int current_split_percentage = 55;
	// == methods 
	// -- user interface workflow 
	private void _components() {
		this.left_tabs = new_dark_tabs();
		this.right_tabs = new_dark_tabs();
        this.current_split_percentage = this.data.split_percentage;
		this.explorer = new_file_explorer_dark_tree(this.data.Directories);
        add_context_menu_item(this.explorer, "Open File/Directory");
		this.help_scintilla = new_scintilla();
		this.help_scintilla.Text = """
// [ Notepad-- ] : A lesser version of Notepad++, don't like it? Use Notepad++ instead.
// ... build on top of Scintilla5.NET ( https://github.com/desjarlais/Scintilla.NET ) by desjarlais

"Use comments like this on top of document to set textmarker highlighting, use comment style of the language used."
{Notepad--;Red:BUG,ISSUE,Close,Titlebar;Yellow:TODO;Silver:SOLVED,Switch}

// Features--
1. "Fixed Dark Theme" // I don't care about your bad taste, it's hardcoded.
2. "Has less language support" // Just use a normal programming language, like a normal person.
3. "Lot of commands using keyboard shortcuts instead of a proper user interface" // Never played a game? Are u a boomer?  
4. "Beeps and Boops Sounds on some keybinds" // Don't know why, it's vibecoded ... 

// Editor Commands
1. Ctrl+N   // New Empty Tab 
2. Ctrl+S   // Save File 
3. Ctrl+H   // Toggle Titlebar ~ Compact Mode 
            // ... use this to access the close button on right corner 
            // ... use this to change window size
4. Ctrl+R   // Toggle readonly mode changing background color : green safe, blue not safe. 
            // It's updates the autocomplete words 

// Editor Commands || Outlining/Folding
1. Alt+O    // Toggle Folding/Outlining 
2. Alt+P    // Fold All Markers Below Current Level 
3. Alt+0    // Fold All ...

// Editor Commands || Defaults from Scintilla
1. Ctrl+Arrows  // Scroll the Editor 
2. Ctrl+Tab     // Change and Focus on Tabs, 
                // ... with tabs focused you can change tabs using arrow keys 
                // ... and shift tab to change between panels.
                // ... tab to return to Editors 

// Editor Commands || Misc
1. Alt+S            // Switch Select Tabs Between Views 
2. Alt+A / Alt+D    // Change the splitter position 
3. Ctrl+F           // Go To Next Selection Match
4. Ctrl+D           // Go To Previous Selection Match
5. Ctrl+Q           // Comment out selected lines 

// Tab Commands
1. Right Click on Tab       // Switch the tab between panels
2. Drag Tab                 // Move the window 
3. Ctrl+Right Click on Tab  // Close the tab 

// Explorer Tab Commands 
1. Insert on File Explorer Tab // Add the selected directory as shortcut 
2. Delete on File Explorer Tab // Remove the selected directory from explorer tree 
3. Enter on File Explorer Tab  // Open the File if the language is supported 
4. Right Click                 // Context Menu 
5. Right Arrow                 // Open+Refresh Folder Contents 

// General Commands 
1. RCtrl // See through window 

>>> 

""";
        this.message_overlay = new OverlayForm(this);
	}
	private void _layout() {
		this.StartPosition = FormStartPosition.Manual;
		SBR_set_location_and_position_from_data();
		// tabs
		add_new_tab(this.left_tabs, this.help_scintilla , "?" );
		add_new_tab(this.right_tabs, this.explorer, "[ File Explorer ]");
		foreach(var file in this.data.LeftFiles) {
			add_new_scintilla_tab(this.left_tabs, file);
		}
		foreach(var file in this.data.RightFiles) {
			add_new_scintilla_tab(this.right_tabs, file);
		}
		// main panel 
		this.main_panel = new_vertical_split(this.left_tabs, this.right_tabs);
		this.main_panel.Dock = DockStyle.Fill;
		this.Controls.Add(this.main_panel);
        set_splitter_distance(this.main_panel, this.current_split_percentage);
	}
	private void _logic() {
        // main form 
        this.DoubleBuffered = true;
		this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.FormClosing += (s, e) => {
			SBR_save_session_data();
		};
		this.Resize += (s,e) => {
            set_splitter_distance(this.main_panel, this.current_split_percentage);
        }; 
        key_shortcut(this, "ctrl","h", () => {
			SBR_compact_toggle();
		});
		key_shortcut(this, "ctrl","n", () => {
			add_new_scintilla_tab(this.left_tabs);
		}); 
		key_shortcut(this, "alt","s", () => {
            var RST = this.right_tabs.SelectedTab;
            var LST = this.left_tabs.SelectedTab;
            switch_view(RST);
            switch_view(LST);
            this.right_tabs.SelectedTab = LST;
            this.left_tabs.SelectedTab = RST; 
        });
        key_shortcut(this, "alt","d", ()=>{
            current_split_percentage += 5;
            set_splitter_distance(this.main_panel, this.current_split_percentage);
        } );
        key_shortcut(this, "alt","a", ()=>{
            current_split_percentage -= 5;
            set_splitter_distance(this.main_panel, this.current_split_percentage);
        } );
        OnTick( this, (s,e) => {
            // opacity logic 
            if ( !is_form_active(this) ) {
                this.Opacity = background_opacity;
            } else if ( IsKeyDown(Keys.RControlKey) ) {
                this.Opacity = background_opacity/2;
            } else {
                this.Opacity = work_opacity;
            }
        }, 100 );
//        key_shortcut(this, "ctrl", Keys.Insert, ()=>{
//            MouseScrollBackWindow(this, true);
//        });
//        key_shortcut(this, "ctrl", Keys.Delete, ()=>{
//            MouseScrollBackWindow(this, false);
//        });
//        OnTick( this, (s,e)=>{
//            if ( get_focused_panel(this.main_panel) == 2 ){
//                if ( current_split_percentage > 40 ) { 
//                    current_split_percentage--; 
//                    set_splitter_distance(this.main_panel, this.current_split_percentage);
//                }
//            } else {
//                if ( current_split_percentage < 60 ) { 
//                    current_split_percentage++; 
//                    set_splitter_distance(this.main_panel, this.current_split_percentage);
//                }
//            }
//        }, 1);

        // to z-order
//        key_shortcut(this, "ctrl", Keys.Insert, ()=>{
//            bring_window_to_least(this);
//        });
//        if ( !register_global_hotkey( Keys.Control, Keys.Delete, ()=>{
//            bring_window_to_top(this);
//        } ) ) { 
//            MessageBox.Show("Failed to Register Hotkey"); 
//        }
        // tabs 
		drag_window(this.left_tabs);
		drag_window(this.right_tabs);
		this.left_tabs.MouseClick += tabs_click_handler;
		this.right_tabs.MouseClick += tabs_click_handler;
		this.right_tabs.ShowToolTips = true;
		this.left_tabs.ShowToolTips = true;
        this.right_tabs.SelectedIndexChanged += (s,e)=>{
            update_border_color(this.right_tabs.SelectedTab);
        };
        this.left_tabs.SelectedIndexChanged += (s,e)=>{
            update_border_color(this.left_tabs.SelectedTab);
        };
		// explorer 
		this.explorer.KeyDown += (s,e) => {
			if (e.KeyCode == Keys.Enter) {
				SBR_open_file_or_dir();
			}
			if (e.KeyCode == Keys.Delete) {
				SBR_rem_selected_dir_to_exp();
				return ;
			}
			if (e.KeyCode == Keys.Insert) {
				SBR_add_selected_dir_to_exp();
				return ;
			}
//			if (e.KeyCode == Keys.Right) {
//				var nodes = this.explorer.GetSelectedNodes();
//				if (nodes.Count == 1) {
//					var node = nodes[0];
//					if (node != null && node.Tag is string path) {
//						if ( is_dir(path) ) {
//							filter_filesystem_node(node, new List<string>() );
//						} 
//					}
//				}
//			}
		}; 
		set_action(this.explorer.ContextMenuStrip, "Open File/Directory", (s,e)=>{
            SBR_open_file_or_dir();
        });
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
        apply_highlight_for_file_directives(this.help_scintilla);
		this.help_scintilla.ReadOnly = true;
	} 
	// -- subroutines 
    // -- subroutines || logic 
    private void tabs_click_handler(Object s, MouseEventArgs e) {
        DarkTabControl tabs = (DarkTabControl) s;
        TabPage pointed_page = get_pointed_tab(tabs, e.Location);
		if (e.Button == MouseButtons.Right)	{	
			if ((Control.ModifierKeys & Keys.Control) != 0) {
				if ( fullpath_scintilla_map.ContainsKey( pointed_page.ToolTipText ) ) { 
					close_tab(tabs, pointed_page);
					fullpath_scintilla_map.Remove( pointed_page.ToolTipText );
					this.data.LeftFiles.Remove(pointed_page.ToolTipText);
					this.data.RightFiles.Remove(pointed_page.ToolTipText);
                    this.message_overlay.Display("Tab Closed : "+pointed_page.Text, 30);
				} 
			} else {
				if (pointed_page == null) return;
				switch_view(pointed_page);
			}
		}			
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
            if (editor.ReadOnly) return ;
            editor.Tag = path;
			string content = editor.Text;
			if ( !is_file(path) ){
				path = save_dialog("txt", this.data.DialogDir);
				if (path == null) return ;
				current_page.ToolTipText = path;
                editor.Tag = path;
			}
            save(path, content); 
//            this.message_overlay.Display("Saved : "+path);
            this.fullpath_scintilla_map[path] = editor;
            current_page.Text = get_filename(path);
            update_border_color(ns, page);
            refresh_style(editor);
//            set_lexer(editor, path);
//            set_folding(editor);
//            set_language_style(editor, path);
//            apply_highlight_for_file_directives(editor);
		});	
		key_shortcut(ns, "ctrl","r", async () => {
            Scintilla editor = ns;
            if (editor.ReadOnly) {
                kill_document_words_async();
            } else {
                _ = get_document_words_async(editor); // fire and forget
            }
//            string content = ns.Text; // testing
            toggle_read_only(editor);
            update_border_color(editor, page);
            refresh_style(editor);
//            string path = page.ToolTipText;
//            set_lexer(editor, path);
//            set_folding(editor);
//            set_language_style(editor, path);
//            apply_highlight_for_file_directives(editor);
		});
		key_shortcut(ns, "alt","z", () => {
//            add_begin_fold_marker(ns);
        });
        key_shortcut(ns, "alt","x", () => {
//            add_end_fold_marker(ns);
        });
        key_shortcut(ns, "alt","c", () => {
//            remove_fold_marker(ns);
        });
        ns.CharAdded += (s, e) => {
            var editor = (Scintilla)s;
            int line_count = -1;
            string? filename = editor.Tag as string;
            if ( !string.IsNullOrWhiteSpace(filename) ) {
                if ( this.fullpath_lines_map.ContainsKey(filename) ) {
                    line_count = this.fullpath_lines_map[filename];
                } else {
                    line_count = editor.Lines.Count;
                }
            }
            if (line_count > 3000) {
                if (char_added_debouncer < (line_count/1000) ) {
                    char_added_debouncer++;
                    return;
                } else {
                    char_added_debouncer = 0;
                }
            }
            if (editor.AutoCActive) return;
            string prefix = get_current_word(editor);
            if (prefix.Length < 2) return;
            string list = build_autocomplete_list(autocomplete_hashset, prefix);
            if (list.Length > 0) editor.AutoCShow(prefix.Length, list);
        };
        
        // DEBUG 
        key_shortcut(ns, "ctrl", Keys.F1, ()=>{
            if ( this.message_overlay==null ) this.message_overlay = new OverlayForm(this);
            this.message_overlay.Display("Testing");
        });
    }
    // -- subroutines || new tabs 
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
		Scintilla editor = new_scintilla();
		this.fullpath_scintilla_map[filename] = editor;
        load_file(editor, filename);
        this.fullpath_lines_map[filename] = editor.Lines.Count;
		TabPage page = add_new_tab(tabs, editor, get_filename(filename) );
		page.ToolTipText = filename;
		// extra logic 
		scintilla_tab_logic(editor, page);
        toggle_read_only(editor);
        update_border_color(editor, page);
        refresh_style(editor);
        fold_all(editor);
		return true;
	}
	private void add_new_scintilla_tab(DarkTabControl tabs) {
		Scintilla ns = new_scintilla();
		TabPage page = add_new_tab(tabs, ns, "New File" );
		scintilla_tab_logic(ns, page);
	}
    // -- subroutines 
    private void SBR_open_file_or_dir() {
        string path = get_selected_filename_from_explorer(this.explorer);
        if ( string.IsNullOrWhiteSpace(path) ) return ;
        if ( is_file(path) ) {
            if ( !add_new_scintilla_tab(this.left_tabs, path) ) return ;
            this.data.LeftFiles.Add(path);
        } else if ( is_dir(path) ) {
            open_in_windows_explorer(path);
        }
    }
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
        this.data.split_percentage = this.current_split_percentage;
		save(
			join(get_exec_dir(), this.data_filename), 
			JsonSerializer.Serialize(this.data)
		);
	}
    private void SBR_update_font() {
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
	private void SBR_compact_toggle() {
		if (hidden_titlebar) {
			show_titlebar(this);
		} else {
			hide_titlebar(this);
		}
		hidden_titlebar = !hidden_titlebar;
	}
    // -- subroutines 
    private Scintilla? get_scintilla(TabPage page) {
        string filename = page.ToolTipText as string; 
        if ( string.IsNullOrWhiteSpace(filename) ) return null;
        if ( !is_file(filename) ) return null;
        if ( !this.fullpath_scintilla_map.ContainsKey(filename) ) return null;
        return this.fullpath_scintilla_map[filename];
    }
    private void repaint_tab(TabPage page) {
        DarkTabControl tabs = page.Parent as DarkTabControl;
        tabs.Invalidate();
    }
    private void update_border_color(TabPage page) {
        Scintilla? ns = get_scintilla(page);
        if (ns==null) return ;
        update_border_color(ns, page);
    }
    private void update_border_color(Scintilla ns, TabPage page) {
        DarkTabControl tabs = page.Parent as DarkTabControl;
        if (ns.ReadOnly) {
            tabs.change_border_color(locked_border_color);
        } else {
            tabs.change_border_color(border_color);
        }
        repaint_tab(page);
    }
    private void refresh_style(Scintilla editor) {
        string path = (string) editor.Tag;
        if (string.IsNullOrWhiteSpace(path)) return;
        if ( !is_code_file(path) ) return; 
        set_lexer(editor, path);
        set_folding(editor);
//        configure_manual_fold_markers(editor);
        set_language_style(editor, path);
        apply_highlight_for_file_directives(editor);
//        apply_fold_marks_for_file_directives(editor);
    }
    private DarkTabControl? get_focused_tab() {
		if ( this.left_tabs.ContainsFocus ) return this.left_tabs;
		if ( this.right_tabs.ContainsFocus ) return this.right_tabs;
		return null;
	}
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
    private void toggle_read_only(Scintilla editor) {
		editor.ReadOnly = !editor.ReadOnly;
		if (editor.ReadOnly) {
			for (int i = 0; i < 256; i++) {
				editor.Styles[i].BackColor = locked_background_color;
			}
		} else {
			for (int i = 0; i < 256; i++) {
				editor.Styles[i].BackColor = background_color;
			}
//            refresh_style(editor); 
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
	private void print(string text) {
        // this.help_scintilla.Text
        this.help_scintilla.ReadOnly = false;
        this.help_scintilla.AppendText(text+"\n");
        this.help_scintilla.ReadOnly = true;
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
    // -- overrides
//    protected override void OnFormClosing(FormClosingEventArgs e) {
//        unregister_all_hotkeys();
//        base.OnFormClosing(e);
//    }
//    protected override void WndProc(ref Message m) {
//        ProcessHotKeyMessage(ref m);
//        base.WndProc(ref m);
//    }
//    protected override void OnLoad(EventArgs e) {
//        base.OnLoad(e);
//        if ( register_global_hotkey(Keys.Control, Keys.J, () => {
//            MessageBox.Show("Hotkey pressed!");
//        }) ) {
//            MessageBox.Show("Registered!"); 
//        } 
//    }



}

/* BUG/ISSUE : Custom Highlights not been applied consistently - SOLVED



*/

// -- END 








