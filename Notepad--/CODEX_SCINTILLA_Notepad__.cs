// -- text marker highlight - tags example 
// {Notepad--T;red:ISSUE;yellow:DEPRECATED,TESTING,PLACEHOLDER,INCOMPLETE;silver:FIXED,REVISION;cyan:TODO,>>>,<<<}
// {Notepad--T;Cyan:Inventory;Silver:Logic,Dialetic;lightgreen:Workflow} 
// {Notepad--T;magenta:methods,attributes,variables}
// {Notepad--T;lightgreen:debug, interception}
// {Notepad--H;1:silver;2:lightblue}

// -- search tokens 
// {Notepad--T;blue:set_lexer,set_language_style}
// {Notepad--S:}

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
using ScintillaNET;
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
using static Codex.Incantation_MOUSE;
// -- ambiguities
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using BorderStyle = System.Windows.Forms.BorderStyle;
using TabDrawMode = System.Windows.Forms.TabDrawMode;
using Timer = System.Windows.Forms.Timer; 
using NM = ScintillaNET.NativeMethods;
//using FoldLevel = ScintillaNET.FoldLevel;

// ===================================== incantation 
// ... graphical user interface and user interaction 
public static class Incantation_SCINTILLA {
    // TODO
    public static bool is_file_modified(Scintilla editor, string filename) {
        // check if the editor.Text match the content in filename 
        return false;
    }
    // variables -- shared colors
    // variables || background, locked_background, foreground 
    public static Color foreground_color = Color.FromArgb(255, 255, 255); // white
    public static Color background_color = Color.FromArgb(10, 10, 12); // black slight blue
    public static Color locked_background_color = Color.FromArgb(0, 5, 0); // black slight green
    // variables || folding and margin
    private static Color fold_fore_color = Color.FromArgb(60, 60, 60); // gray 
    private static Color fold_back_color = Color.FromArgb(255, 255, 255); // white 
    private static Color margin_fore_color = Color.FromArgb(120,120,120); // gray
    private static Color margin_back_color = Color.FromArgb(30,30,30); // dark gray 
    // variables || language specific style
    private static Color default_word_color = Color.White;
    private static Color keyword1_color = Color.Silver; //Color.FromArgb(115, 187, 255); // orange 
    private static Color keyword1_backcolor;
    private static Color keyword2_color = Color.LightBlue; // Color.FromArgb(0, 255, 95); // green 
    private static Color keyword2_backcolor; 
    private static Color comment_fore_color = Color.FromArgb(0, 255, 153); // Green
    private static Color comment_back_color = Color.FromArgb(0, 51, 0); // Dark Green
    private static Color number_fore_color = Color.Cyan;
    private static Color number_back_color = Color.FromArgb(0, 0, 73); // dark blue 
    private static Color string_fore_color = Color.Red; 
    private static Color string_back_color = Color.FromArgb(60, 10, 10); // gray
    private static Color operator_color = Color.Yellow;
    private static Color preprocessor_color = Color.Gray;
    // variables | methods -- shared colors
    public static void reset_global_dark_theme_colors() {
        foreground_color = Color.FromArgb(255, 255, 255); // white
        background_color = Color.FromArgb(10, 10, 12); // black slight blue
        locked_background_color = Color.FromArgb(0, 5, 0); // black slight green
        fold_fore_color = Color.FromArgb(60, 60, 60); // gray 
        fold_back_color = Color.FromArgb(255, 255, 255); // white 
        margin_fore_color = Color.FromArgb(120,120,120); // gray
        margin_back_color = Color.FromArgb(30,30,30); // dark gray 
        // -- 
        default_word_color = Color.White;
        keyword1_color = Color.Silver; //Color.FromArgb(115, 187, 255); // orange 
        keyword2_color = Color.LightBlue; //Color.FromArgb(0, 255, 95); // green 
        comment_fore_color = Color.FromArgb(0, 255, 153); // Green
        comment_back_color = Color.FromArgb(0, 51, 0); // Dark Green
        number_fore_color = Color.Cyan;
        number_back_color = Color.FromArgb(0, 0, 73); // dark blue 
        string_fore_color = Color.Red; //Color.FromArgb(230, 200, 0); // Color.FromArgb(230, 230, 230); // white
        string_back_color = Color.FromArgb(60, 10, 10); // gray
        operator_color = Color.Yellow;
        preprocessor_color = Color.Gray;
    }
//    public static void neon_color(string color_name, ref Style style) { // TESTING
//        switch(color_name){
//            case "green":
//                neon_green(ref style.ForeColor, ref style.BackColor);
//                break;
//            case "red":
//                neon_red(ref style.ForeColor, ref style.BackColor);
//                break;
//            case "blue":
//                neon_blue(ref style.ForeColor, ref style.BackColor);
//                break;
//            case "gray":
//                neon_gray(ref style.ForeColor, ref style.BackColor);
//                break;
//            case "yellow":
//                neon_yellow(ref style.ForeColor, ref style.BackColor);
//                break;
//            case "purple":
//                neon_purple(ref style.ForeColor, ref style.BackColor);
//                break;
//            default:
//                neon_green(ref style.ForeColor, ref style.BackColor);
//                break;
//        }
//    }
    public static void neon_green(ref Color fore,ref Color back) {
        fore = Color.FromArgb(0, 255, 153);
        back = Color.FromArgb(0, 51, 0);
    }
    public static void neon_red(ref Color fore,ref Color back) {
        fore = Color.Red;
        back = Color.FromArgb(60, 10, 10);
    }
    public static void neon_blue(ref Color fore,ref Color back) {
        fore = Color.Cyan;
        back = Color.FromArgb(0, 0, 73);
    }
    public static void neon_gray(ref Color fore,ref Color back) {
        fore = Color.LightGray;
        back = Color.FromArgb(40,40,40);
    }
    public static void neon_yellow(ref Color fore, ref Color back) {
        fore = Color.FromArgb(255, 255, 102);   // bright neon yellow
        back = Color.FromArgb(51, 51, 0);       // deep muted yellow/olive background
    }
    public static void neon_purple(ref Color fore, ref Color back) {
        fore = Color.FromArgb(204, 102, 255);   // neon purple/magenta
        back = Color.FromArgb(32, 0, 51);       // dark purple background
    }
    // Logic [ new_scintilla ] 
    // -> new_scintilla() || ... | init_dark_theme_scintilla() | set_keyshortcuts() || clear_cmd_keys() | key_shortcut()
    // methods -- factory
	public static Scintilla new_scintilla() {
		var editor = new Scintilla();
		editor.Dock = DockStyle.Fill;
		editor.BorderStyle = ScintillaNET.BorderStyle.None;
		editor.CaretWidth = 2;
		editor.CaretLineVisible = true;
		editor.Margins[0].Type = MarginType.Number;
		editor.Margins[0].Width = 40;
		editor.EdgeMode = EdgeMode.Line;
		editor.EdgeColumn = 120;
        editor.WrapMode = WrapMode.Word;
        editor.WrapIndentMode = WrapIndentMode.Indent;
        editor.AutoCIgnoreCase = true;
        editor.AutoCMaxHeight = 30;
        editor.AutoCSeparator = ' ';
        init_dark_theme_scintilla(editor);
        set_keyshortcuts(editor);
		return editor;
	}
	public static void load_file(Scintilla editor, string filename) { // REVISION
		if ( string.IsNullOrWhiteSpace(filename) ) return ;
        if ( !is_file(filename) ) return ;
        bool b_or_ed_ro = editor.ReadOnly;
        editor.ReadOnly = true;
		string? text = load(filename); 
		
		if (string.IsNullOrWhiteSpace(text)) { 
            editor.ReadOnly = b_or_ed_ro;
            return ;
        }
		editor.ReadOnly = false;
		editor.Text = text;
        editor.Tag = filename;
        editor.ReadOnly = b_or_ed_ro;
	}
	public static void init_dark_theme_scintilla(Scintilla editor) { // REVISION 
        editor.StyleResetDefault();
		editor.Styles[Style.Default].Font = "Consolas";
		editor.Styles[Style.Default].Size = 8;
		editor.Styles[Style.Default].Bold = true;
		editor.Styles[Style.Default].ForeColor = foreground_color;
		editor.Styles[Style.Default].BackColor = background_color;
        editor.StyleClearAll();
        editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120,120,120);
		editor.Styles[Style.LineNumber].BackColor = margin_back_color;
        editor.Styles[Style.IndentGuide].ForeColor = Color.FromArgb(60,60,60);
        editor.EdgeColor = Color.FromArgb(60,60,60);
        editor.CaretForeColor = Color.White;
        editor.CaretLineBackColor = Color.FromArgb(40, 40, 40);
        editor.SetSelectionBackColor(true, Color.FromArgb(70, 70, 70));
		editor.SetSelectionForeColor(true, Color.White);
    }
    public static void clear_cmd_keys(Scintilla editor) { // TODO ISSUE
        editor.ClearCmdKey(Keys.Control | Keys.F);
        editor.ClearCmdKey(Keys.Control | Keys.D);
        editor.ClearCmdKey(Keys.Control | Keys.Q);
        editor.ClearCmdKey(Keys.Control | Keys.S);
        editor.ClearCmdKey(Keys.Control | Keys.A);
    }
    public static void set_keyshortcuts(Scintilla editor) { // TESTING
        clear_cmd_keys(editor);
        key_shortcut(editor, "alt", Keys.D0, ()=>{fold_all(editor);});
		key_shortcut(editor, "alt", "p", ()=>{smart_fold_all(editor);});
		key_shortcut(editor, "alt", "o", ()=>{toggle_fold_marker(editor);});
        key_shortcut(editor, "ctrl", "d", ()=>{
            bool b_read_only_back = editor.ReadOnly;
            string token = get_selected_token(editor);
            if ( string.IsNullOrWhiteSpace(token) ) { 
                editor.ReadOnly = true;
                token =  input_dialog(null, "Find Previous", "Input Token", "");
                editor.ReadOnly = b_read_only_back;
            }
            if ( string.IsNullOrWhiteSpace(token) ) return ;
            find_prev_token(editor, token);
        });
        key_shortcut(editor, "ctrl", "f", ()=>{
            bool b_read_only_back = editor.ReadOnly;
            string token = get_selected_token(editor);
            if ( string.IsNullOrWhiteSpace(token) ) { 
                editor.ReadOnly = true;
                token =  input_dialog(null, "Find Next", "Input Token", "");
                editor.ReadOnly = b_read_only_back;
            }
            if ( string.IsNullOrWhiteSpace(token) ) return ;
            find_next_token(editor, token);
        });
        key_shortcut(editor, "ctrl", "q", ()=>{
            toggle_comment_lines(editor); 
        });
	}
    // methods -- lexer, folding and style
    public static bool set_lexer(Scintilla editor, string filename) {
        if ( string.IsNullOrWhiteSpace(filename) ) return false;
		string ext = Path.GetExtension(filename).ToLowerInvariant();
        if ( string.IsNullOrWhiteSpace(ext) ) return false;
        // -- hand picked lexer names which will mismatch extension names 
        switch (ext) {
            case ".md":
                editor.LexerName = "markdown";
                return true;
            case ".html":
            case ".htm":
            case ".csproj":
                editor.LexerName = "hypertext";
                return true;
            case ".ahk":
                editor.LexerName = "cpp";
                return true;
            case ".c":
            case ".h":
            case ".hpp":
            case ".cs":
            case ".java":
            case ".ts":
            case ".js":
                editor.LexerName = "cpp";
                return true;
            case ".bat":
                editor.LexerName = "batch";
                return true;
            case ".sh":
                editor.LexerName = "bash";
                return true;
            case ".tex":
                editor.LexerName = "latex";
                return true;
            case ".ps1":
                editor.LexerName = "powershell";
                return true;
            case ".py":
            case ".pyw":
                editor.LexerName = "python";
                return true;
            case ".rb":
                editor.LexerName = "ruby";
                return true;
            case ".adb":
                editor.LexerName = "ada";
                return true;
            case ".bb":
                editor.LexerName = "blitzbasic";
                return true;
            case ".f03":
            case ".f95":
            case ".f90":
                editor.LexerName = "fortran";
                return true;
            case ".bas":
            case ".bi":
                editor.LexerName = "freebasic";
                return true;
            case ".lsp":
                editor.LexerName = "lisp";
                return true;
            case ".m":
                editor.LexerName = "matlab";
                return true;
            case ".rs":
                editor.LexerName = "rust";
                return true;
            case ".pas":
            case ".pp":
            case ".p":
            case ".inc":
            case ".lpr":
                editor.LexerName = "pascal";
                return true;
            case ".php":
            case ".php3":
            case ".php4":
            case ".php5":
            case ".phps":
            case ".phpt":
            case ".phtml":
                editor.LexerName = "phpscript";
                return true;
        }
        // -- the lexer_name directly on GetLexerNames iterator
        foreach(string lexer_name in Lexilla.GetLexerNames()) {
            if (ext == "."+lexer_name) {
                editor.LexerName = lexer_name;
                return true;
            }
        } 
        return false;
    }
    public static void set_folding(Scintilla scintilla, string filename) { 
        if ( string.IsNullOrWhiteSpace(filename) ) return;
        string ext = Path.GetExtension(filename).ToLowerInvariant();
        // Instruct the lexer to calculate folding
		scintilla.SetProperty("fold", "1");
        scintilla.SetProperty("fold.compact", "0"); 
        scintilla.SetProperty("fold.comment", "1");
		// Configure a margin to display folding symbols
		scintilla.Margins[2].Type = MarginType.Symbol;
		scintilla.Margins[2].Mask = Marker.MaskFolders;
		scintilla.Margins[2].Sensitive = true;
		scintilla.Margins[2].Width = 20;
		// Set colors for all folding markers
		for (int i = 25; i <= 31; i++) {
			scintilla.Markers[i].SetForeColor(fold_fore_color);
			scintilla.Markers[i].SetBackColor(fold_back_color);
		}
		// Marker colors
		Color fore = fold_fore_color;
		Color back = fold_back_color;
		scintilla.Markers[Marker.Folder].SetForeColor(fore);
		scintilla.Markers[Marker.Folder].SetBackColor(back);
		scintilla.Markers[Marker.FolderOpen].SetForeColor(fore);
		scintilla.Markers[Marker.FolderOpen].SetBackColor(back);
		scintilla.Markers[Marker.FolderEnd].SetForeColor(fore);
		scintilla.Markers[Marker.FolderEnd].SetBackColor(back);
		scintilla.Markers[Marker.FolderMidTail].SetForeColor(fore);
		scintilla.Markers[Marker.FolderMidTail].SetBackColor(back);
		scintilla.Markers[Marker.FolderOpenMid].SetForeColor(fore);
		scintilla.Markers[Marker.FolderOpenMid].SetBackColor(back);
		scintilla.Markers[Marker.FolderSub].SetForeColor(fore);
		scintilla.Markers[Marker.FolderSub].SetBackColor(back);
		scintilla.Markers[Marker.FolderTail].SetForeColor(fore);
		scintilla.Markers[Marker.FolderTail].SetBackColor(back);
		// Folding margin background
		scintilla.SetFoldMarginColor(true, Color.FromArgb(30, 30, 30));
		scintilla.SetFoldMarginHighlightColor(true, Color.FromArgb(30, 30, 30));
		// Configure folding markers with respective symbols
		scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
		scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
		scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
		scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
		scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
		scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
		scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;
        // -- 
        
        // -- some languages don't fold automatically
        if ( !string.IsNullOrWhiteSpace(ext) ) {
            switch (ext) {
                case ".bb":
                    set_indent_folding(scintilla);
                    break;
                case ".html":
                case ".xml":
                case ".csproj":
                    scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
                    set_indent_folding(scintilla);
                    break;
                default:
                    scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
                    break;
            }
        } else {
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }
    }
    public static void set_indent_folding(Scintilla editor) {
        int lineCount = editor.Lines.Count;
        for (int i = 0; i < lineCount; i++) {
            var line = editor.Lines[i];
            string text = line.Text;
            int indent = GetIndentLevel(text);
            // Default level
            int level = NM.SC_FOLDLEVELBASE | indent;
            // Look ahead to detect fold header
            if (i < lineCount - 1) {
                string nextText = editor.Lines[i + 1].Text;
                int nextIndent = GetIndentLevel(nextText);
                if (nextIndent > indent) {
                    level |= NM.SC_FOLDLEVELHEADERFLAG;
                }
            }
            // Mark empty lines properly
            if (string.IsNullOrWhiteSpace(text)) level |= NM.SC_FOLDLEVELWHITEFLAG;
            line.FoldLevel = level;
        }
    }
    private static int GetIndentLevel(string text) {
        int count = 0;
        foreach (char c in text) {
            if (c == ' ') count++;
            else if (c == '\t') count += 4; // or editor.TabWidth
            else break;
        }
        return count;
    }

	// variables -- styling 
    // -- keyword ~ NAME_KEYWORDS1_LIST_MAP ~ index 0 on scintilla functions 
    // -- keyword 2 ~ NAME_KEYWORDS2_LIST_MAP ~ index 1 on scintilla functions 
    public static Dictionary<string, List<string>> NAME_KEYWORDS1_LIST_MAP = new Dictionary<string, List<string>>(); 
    public static Dictionary<string, List<string>> NAME_KEYWORDS2_LIST_MAP = new Dictionary<string, List<string>>(); 
    public static Dictionary<string, List<string>> NAME_KEYWORDS3_LIST_MAP = new Dictionary<string, List<string>>(); 
    public static Dictionary<string, List<string>> NAME_KEYWORDS4_LIST_MAP = new Dictionary<string, List<string>>(); 
    public static Dictionary<string, List<string>> NAME_KEYWORDS5_LIST_MAP = new Dictionary<string, List<string>>(); 
    // -- 
    private static Dictionary<string,string> NAME_KEYWORDS1_STR_MAP = new Dictionary<string,string>(); 
    private static Dictionary<string,string> NAME_KEYWORDS2_STR_MAP = new Dictionary<string,string>(); 
    private static Dictionary<string,string> NAME_KEYWORDS3_STR_MAP = new Dictionary<string,string>(); 
    private static Dictionary<string,string> NAME_KEYWORDS4_STR_MAP = new Dictionary<string,string>(); 
    private static Dictionary<string,string> NAME_KEYWORDS5_STR_MAP = new Dictionary<string,string>(); 
    // variables | methods -- styling 
    public static void set_language_style(Scintilla scintilla, string filename) { // INCOMPLETE
		if ( string.IsNullOrWhiteSpace(filename) ) return ;
		string ext = filename;
		if (filename.Contains(".")) ext = Path.GetExtension(filename);
		ext = ext.ToLower();
		switch (ext) {
            case ".ahk":
                set_ahk_style(scintilla);
                break;
            case ".c":
                set_c_style(scintilla);
                break;
            case ".h":
            case ".hpp":
			case ".cpp":
				set_cpp_style(scintilla);
				break;
			case ".cs":
				set_cs_style(scintilla);
				break;
            case ".pyw":
            case ".py":
                set_py_style(scintilla);
                break;
            case ".java":
                set_java_style(scintilla);
                break;
            case ".lua":
                set_lua_style(scintilla);
                break;
            case ".js":
                set_javascript_style(scintilla);
                break;
            case ".bat":
                set_batch_style(scintilla);
                break;
            case ".sh":
                set_bash_style(scintilla);
                break;
            case ".htm":
            case ".html":
            case ".xml":
            case ".csproj":
                set_html_style(scintilla);
                break;
            case ".ps1":
                set_powershell_style(scintilla);
                break;
            case ".ada":
            case ".adb":
                set_ada_style(scintilla);
                break;
            case ".bb":
                set_blitzbasic_style(scintilla);
                break;
            case ".css":
                set_css_style(scintilla);
                break;
            case ".f03":
            case ".f95":
            case ".f90":
                set_fortran_style(scintilla);
                break;
            case ".bas":
            case ".bi":
                set_freebasic_style(scintilla);
                break;
            case ".json":
                set_json_style(scintilla);
                break; 
            case ".lisp":
            case ".lsp":
                set_lisp_style(scintilla);
                break;
            case ".m":
                set_matlab_style(scintilla);
                break;
            case ".rs":
                set_rust_style(scintilla);
                break;
            case ".pas":
            case ".pp":
            case ".p":
            case ".inc":
            case ".lpr":
                set_pascal_style(scintilla);
                break;
            case ".php":
            case ".php3":
            case ".php4":
            case ".php5":
            case ".phps":
            case ".phpt":
            case ".phtml":
                set_php_style(scintilla);
                break;
		}
	}
    // Logic [ update_keywords_str_from_lst ] 
    // U := update_keywords_str_from_lst
    // N := need update
    // -> U() || %* dicts not contains key | % N() || joins with space the keywords 1 and 2 from list 
    // -> N() || %* dicts not contains key || return true 
    // -> N() || %* dicts not contains key | calculate the size of list with space and compare with size of string already there | % size don't match || return true 
    // -> N() || %* dicts not contains key | calculate the size of list with space and compare with size of string already there | % size don't match | % else || return false 
    private static bool need_update_str_keywords1(string name) { 
        if ( !NAME_KEYWORDS1_LIST_MAP.ContainsKey(name) ) return false; 
        if ( !NAME_KEYWORDS1_STR_MAP.ContainsKey(name) ) return true; 
        var LIST = NAME_KEYWORDS1_LIST_MAP[name]; 
        var STR = NAME_KEYWORDS1_STR_MAP[name];
        int len = LIST.Count;
        if (len<1) return false;
        int string_len_sum = 0;
        foreach(var s in LIST) string_len_sum += s.Length;
        string_len_sum += len-1;
        return string_len_sum!=STR.Length;
    }
    private static bool need_update_str_keywords2(string name) { 
        if ( !NAME_KEYWORDS2_LIST_MAP.ContainsKey(name) ) return false; 
        if ( !NAME_KEYWORDS2_STR_MAP.ContainsKey(name) ) return true; 
        var LIST = NAME_KEYWORDS2_LIST_MAP[name]; 
        var STR = NAME_KEYWORDS2_STR_MAP[name];
        int len = LIST.Count;
        if (len<1) return false;
        int string_len_sum = 0;
        foreach(var s in LIST) string_len_sum += s.Length;
        string_len_sum += len-1;
        return string_len_sum!=STR.Length;
    }
    private static bool update_keywords_str_from_lst(string name) { 
        if ( !NAME_KEYWORDS1_LIST_MAP.ContainsKey(name) ) return false; // needs initialization 
        if ( !NAME_KEYWORDS2_LIST_MAP.ContainsKey(name) ) return false; // needs initialization 
        if ( need_update_str_keywords1(name) ) {
            NAME_KEYWORDS1_STR_MAP[name] = string.Join(" ",NAME_KEYWORDS1_LIST_MAP[name]);
        }
        if ( need_update_str_keywords2(name) ) {
            NAME_KEYWORDS2_STR_MAP[name] = string.Join(" ",NAME_KEYWORDS2_LIST_MAP[name]);
        }
        return true; 
    }
    private static void update_keywords(Scintilla editor, string name, string keywords1, string keywords2) { 
        if ( !update_keywords_str_from_lst(name) ) { 
            // only if not initialized 
            NAME_KEYWORDS1_LIST_MAP[name] = new List<string>{keywords1};
            NAME_KEYWORDS2_LIST_MAP[name] = new List<string>{keywords2};
            update_keywords_str_from_lst(name);
        }
        string key1 = NAME_KEYWORDS1_STR_MAP[name];
        string key2 = NAME_KEYWORDS2_STR_MAP[name];
        if ( !string.IsNullOrWhiteSpace(key1) ) editor.SetKeywords(0,key1);
        if ( !string.IsNullOrWhiteSpace(key2) ) editor.SetKeywords(1,key2);
    }
    
    // >>>
    private static void update_keywords( // TODO INCOMPLETE 
        Scintilla editor, 
        string name, 
        string keywords1, 
        string keywords2, 
        string keywords3
    ) { 
        update_keywords(editor, name, keywords1, keywords2); // fallback 
    }
    private static void update_keywords( // TODO INCOMPLETE 
        Scintilla editor, 
        string name, 
        string keywords1, 
        string keywords2, 
        string keywords3,
        string keywords4
    ) { 
        update_keywords(editor, name, keywords1, keywords2); // fallback 
    }
    // <<<
    
    public static void set_py_style(Scintilla editor) { 
        editor.Styles[Style.Python.Default].ForeColor = default_word_color;
        editor.Styles[Style.Python.CommentLine].ForeColor = comment_fore_color;
        editor.Styles[Style.Python.CommentLine].BackColor = comment_back_color;
        editor.Styles[Style.Python.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Python.Number].BackColor = number_back_color;
        editor.Styles[Style.Python.String].ForeColor = string_fore_color;
        editor.Styles[Style.Python.String].BackColor = string_back_color;
        editor.Styles[Style.Python.Character].ForeColor = string_fore_color;
        editor.Styles[Style.Python.Character].BackColor = string_back_color;
        editor.Styles[Style.Python.Triple].ForeColor = string_fore_color;
        editor.Styles[Style.Python.Triple].BackColor = string_back_color;
        editor.Styles[Style.Python.TripleDouble].ForeColor = string_fore_color;
        editor.Styles[Style.Python.TripleDouble].BackColor = string_back_color;
        editor.Styles[Style.Python.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Python.Word2].ForeColor = keyword2_color;
        editor.Styles[Style.Python.Operator].ForeColor = operator_color;
        update_keywords(
            editor,
            "python",
            "and as assert async await break class continue def del elif else except False finally for from global if import in is lambda None nonlocal not or pass raise return True try while with yield",
            "int float str bool list tuple dict set bytes object type"
        );
    }
    public static void set_java_style(Scintilla editor) { 
        set_c_family_style(editor);
        update_keywords(
            editor,
            "java",
            "abstract assert break case catch class const continue default do else enum extends final finally for goto if implements import instanceof interface native new package private protected public return strictfp super switch synchronized this throw throws transient try volatile while",
            "boolean byte char double float int long short void String Object Class"
        );
    }
    public static void set_javascript_style(Scintilla editor) { 
        set_c_family_style(editor);
        update_keywords(
            editor,
            "javascript",
            "break case catch class const continue debugger default delete do else export extends finally for function if import in instanceof let new return super switch this throw try typeof var void while with yield",
            "Array Boolean Date Error Function JSON Math Number Object Promise RegExp String Map Set WeakMap WeakSet Symbol BigInt console window document"
        );
    }
    public static void set_lua_style(Scintilla editor) { 
        editor.Styles[Style.Lua.Default].ForeColor = default_word_color;
        editor.Styles[Style.Lua.Comment].ForeColor = comment_fore_color;
        editor.Styles[Style.Lua.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Lua.CommentLine].ForeColor = comment_fore_color;
        editor.Styles[Style.Lua.CommentLine].BackColor = comment_back_color;
        editor.Styles[Style.Lua.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Lua.Number].BackColor = number_back_color;
        editor.Styles[Style.Lua.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Lua.Word2].ForeColor = keyword2_color;
        editor.Styles[Style.Lua.String].ForeColor = string_fore_color;
        editor.Styles[Style.Lua.String].BackColor = string_back_color;
        editor.Styles[Style.Lua.Character].ForeColor = string_fore_color;
        editor.Styles[Style.Lua.Character].BackColor = string_back_color;
        editor.Styles[Style.Lua.LiteralString].ForeColor = string_fore_color;
        editor.Styles[Style.Lua.LiteralString].BackColor = string_back_color;
        editor.Styles[Style.Lua.Operator].ForeColor = operator_color;
        editor.Styles[Style.Lua.Preprocessor].ForeColor = preprocessor_color;
        update_keywords(
            editor,
            "lua",
            "and break do else elseif end false for function goto if in local nil not or repeat return then true until while",
            "assert collectgarbage dofile error getmetatable ipairs load loadfile next pairs pcall print rawequal rawget rawlen rawset require select setmetatable tonumber tostring type xpcall"
        );
    }
    public static void set_bash_style(Scintilla editor) { 
        set_bash_common_style(editor);
        update_keywords(
            editor,
            "bash",
            "7z adduser alias apt-get ar as asa autoconf automake awk banner base64 basename bash bc bdiff blkid break bsdcpio bsdtar bunzip2 bzcmp bzdiff bzegrep bzfgrep bzgrep bzip2 bzip2recover bzless bzmore c++ cal calendar case cat cc cd cfdisk chattr chgrp chmod chown chroot chvt cksum clang clang++ clear cmp col column comm compgen compress continue convert cp cpio crontab crypt csplit ctags curl cut date dc dd deallocvt declare deluser depmod deroff df dialog diff diff3 dig dircmp dirname disown dmesg do done dpkg dpkg-deb du echo ed egrep elif else env esac eval ex exec exit expand export expr fakeroot false fc fdisk ffmpeg fgrep fi file find flex flock fmt fold for fsck function functions fuser fusermount g++ gas gawk gcc gdb genisoimage getconf getopt getopts git gpg gpgsplit gpgv grep gres groff groups gunzip gzexe hash hd head help hexdump hg history httpd iconv id if ifconfig ifdown ifquery ifup in insmod integer inxi ip ip6tables ip6tables-save ip6tables-restore iptables iptables-save iptables-restore ip jobs join kill killall killall5 lc ld ldd let lex line ln local logname look ls lsattr lsb_release lsblk lscpu lshw lslocks lsmod lsusb lzcmp lzegrep lzfgrep lzgrep lzless lzma lzmainfo lzmore m4 mail mailx make man mkdir mkfifo mkswap mktemp modinfo modprobe mogrify more mount msgfmt mt mv nameif nasm nc ndisasm netcat newgrp nl nm nohup ntps objdump od openssl p7zip pacman passwd paste patch pathchk pax pcat pcregrep pcretest perl pg ping ping6 pivot_root poweroff pr print printf ps pwd python python2 python3 ranlib read readlink readonly reboot reset return rev rm rmdir rmmod rpm rsync sed select service set sh sha1sum sha224sum sha256sum sha3sum sha512sum shift shred shuf shutdown size sleep sort spell split start stop strings strip stty su sudo sum suspend switch_root sync systemctl tac tail tar tee test then time times touch tr trap troff true tsort tty type typeset ulimit umask umount unalias uname uncompress unexpand uniq unlink unlzma unset until unzip unzipsfx useradd userdel uudecode uuencode vi vim wait wc wget whence which while who wpaste wstart xargs xdotool xxd xz xzcat xzcmp xzdiff xzfgrep xzgrep xzless xzmore yes yum zcat zcmp zdiff zegrep zfgrep zforce zgrep zless zmore znew zsh",
            ""
        );
    }
    public static void set_html_style(Scintilla editor) { // ISSUE TODO INCOMPLETE - autofolding don't work
        editor.Styles[Style.Html.Default].ForeColor = default_word_color;
        editor.Styles[Style.Html.Tag].ForeColor = keyword1_color;
        editor.Styles[Style.Html.TagUnknown].ForeColor = default_word_color;
        editor.Styles[Style.Html.Attribute].ForeColor = keyword2_color;
        editor.Styles[Style.Html.AttributeUnknown].ForeColor = default_word_color;
        editor.Styles[Style.Html.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Html.Number].BackColor = number_back_color;
        editor.Styles[Style.Html.DoubleString].ForeColor = string_fore_color;
        editor.Styles[Style.Html.DoubleString].BackColor = string_back_color;
        editor.Styles[Style.Html.SingleString].ForeColor = string_fore_color;
        editor.Styles[Style.Html.SingleString].BackColor = string_back_color;
        editor.Styles[Style.Html.Other].ForeColor = operator_color;
//        editor.Styles[Style.Html.Other].BackColor = string_back_color;
        editor.Styles[Style.Html.Comment].ForeColor = comment_fore_color;
        editor.Styles[Style.Html.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Html.Entity].ForeColor = Color.LightGreen;
        editor.Styles[Style.Html.TagEnd].ForeColor = default_word_color;
        editor.Styles[Style.Html.XmlStart].ForeColor = default_word_color;
        editor.Styles[Style.Html.XmlEnd].ForeColor = default_word_color;
        editor.Styles[Style.Html.Script].ForeColor = default_word_color;
        editor.Styles[Style.Html.Asp].ForeColor = default_word_color;
        editor.Styles[Style.Html.AspAt].ForeColor = default_word_color;
        editor.Styles[Style.Html.CData].ForeColor = default_word_color;
        editor.Styles[Style.Html.Question].ForeColor = default_word_color;
        editor.Styles[Style.Html.Value].ForeColor = default_word_color;
        editor.Styles[Style.Html.XcComment].ForeColor = default_word_color;
        update_keywords(
            editor,
            "html",
            "^data- a abbr accept accept-charset accesskey acronym action address align alink alt applet archive area article aside async audio autocomplete autofocus axis b background base basefont bdi bdo bgcolor bgsound big blink blockquote body border br button canvas caption cellpadding cellspacing center char charoff charset checkbox checked cite class classid clear code codebase codetype col colgroup color cols colspan command compact content contenteditable contextmenu coords data datafld dataformatas datalist datapagesize datasrc datetime dd declare defer del details dfn dialog dir disabled div dl draggable dropzone dt element em embed enctype event face fieldset figcaption figure file font footer for form formaction formenctype formmethod formnovalidate formtarget frame frameborder frameset h1 h2 h3 h4 h5 h6 head header headers height hgroup hidden hr href hreflang hspace html http-equiv i id iframe image img input ins isindex ismap kbd keygen label lang language leftmargin legend li link list listing longdesc main manifest map marginheight marginwidth mark marquee max maxlength media menu menuitem meta meter method min minlength multicol multiple name nav nobr noembed noframes nohref noresize noscript noshade novalidate nowrap object ol onabort onafterprint onautocomplete onautocompleteerror onbeforeonload onbeforeprint onblur oncancel oncanplay oncanplaythrough onchange onclick onclose oncontextmenu oncuechange ondblclick ondrag ondragend ondragenter ondragleave ondragover ondragstart ondrop ondurationchange onemptied onended onerror onfocus onhashchange oninput oninvalid onkeydown onkeypress onkeyup onload onloadeddata onloadedmetadata onloadstart onmessage onmousedown onmouseenter onmouseleave onmousemove onmouseout onmouseover onmouseup onmousewheel onoffline ononline onpagehide onpageshow onpause onplay onplaying onpointercancel onpointerdown onpointerenter onpointerleave onpointerlockchange onpointerlockerror onpointermove onpointerout onpointerover onpointerup onpopstate onprogress onratechange onreadystatechange onredo onreset onresize onscroll onseeked onseeking onselect onshow onsort onstalled onstorage onsubmit onsuspend ontimeupdate ontoggle onundo onunload onvolumechange onwaiting optgroup option output p param password pattern picture placeholder plaintext pre profile progress prompt public q radio readonly rel required reset rev reversed role rows rowspan rp rt rtc ruby rules s samp sandbox scheme scope scoped script seamless section select selected shadow shape size sizes small source spacer span spellcheck src srcdoc srcset standby start step strike strong style sub submit summary sup svg svg:svg tabindex table target tbody td template text textarea tfoot th thead time title topmargin tr track tt type u ul usemap valign value valuetype var version video vlink vspace wbr width xml xmlns xmp",
            "ATTLIST DOCTYPE ELEMENT ENTITY NOTATION"
        );
    }
    public static void set_powershell_style(Scintilla editor) { 
        editor.Styles[Style.PowerShell.Default].ForeColor = default_word_color;
        editor.Styles[Style.PowerShell.Comment].ForeColor = comment_fore_color;
        editor.Styles[Style.PowerShell.Comment].BackColor = comment_back_color;
        editor.Styles[Style.PowerShell.Number].ForeColor = number_fore_color;
        editor.Styles[Style.PowerShell.Number].BackColor = number_back_color;
        editor.Styles[Style.PowerShell.Keyword].ForeColor = keyword1_color;
        // Strings
        editor.Styles[Style.PowerShell.String].ForeColor = string_fore_color;
        editor.Styles[Style.PowerShell.String].BackColor = string_back_color;
        editor.Styles[Style.PowerShell.Operator].ForeColor = operator_color;
        // Variables
        editor.Styles[Style.PowerShell.Variable].ForeColor = Color.LightBlue;
        // Commands (cmdlets/functions)
        editor.Styles[Style.PowerShell.Cmdlet].ForeColor = keyword2_color;
        update_keywords(
            editor,
            "powershell",
            "begin break catch class continue data do dynamicparam else elseif end enum exit filter finally for foreach function hidden if in inlinescript parallel param process return sequence static switch throw trap try until using while workflow",
            ""
        );
    }    
    public static void set_c_family_style(Scintilla editor) { 
		// Configure the CPP (C#) lexer styles
		editor.Styles[Style.Cpp.Default].ForeColor = default_word_color;
		editor.Styles[Style.Cpp.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.Cpp.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Cpp.CommentDoc].ForeColor = comment_fore_color;
		editor.Styles[Style.Cpp.CommentDoc].BackColor = comment_back_color;
		editor.Styles[Style.Cpp.CommentLine].ForeColor = comment_fore_color;
		editor.Styles[Style.Cpp.CommentLine].BackColor = comment_back_color;
		editor.Styles[Style.Cpp.CommentLineDoc].ForeColor = comment_fore_color;
		editor.Styles[Style.Cpp.CommentLineDoc].BackColor = comment_back_color;
		editor.Styles[Style.Cpp.Number].ForeColor = number_fore_color;
		editor.Styles[Style.Cpp.Number].BackColor = number_back_color;
        editor.Styles[Style.Cpp.Word].ForeColor = keyword1_color;
		editor.Styles[Style.Cpp.Word2].ForeColor = keyword2_color;
        editor.Styles[Style.Cpp.GlobalClass].ForeColor = Color.FromArgb(255, 77, 77); 
//        editor.Styles[Style.Cpp.Identifier].ForeColor = Color.Green;       
//        editor.Styles[Style.Cpp.Uuid].ForeColor = Color.Brown; 
//        editor.Styles[Style.Cpp.CommentDocKeyword].ForeColor = Color.DarkRed;     // Set 6
//        editor.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = Color.DarkMagenta; // Set 7
		editor.Styles[Style.Cpp.String].ForeColor = string_fore_color;
		editor.Styles[Style.Cpp.String].BackColor = string_back_color;
//        editor.Styles[Style.Cpp.StringRaw].ForeColor = string_fore_color;
//		editor.Styles[Style.Cpp.StringRaw].BackColor = string_back_color;
		editor.Styles[Style.Cpp.Character].ForeColor = string_fore_color;
		editor.Styles[Style.Cpp.Character].BackColor = string_back_color;
		editor.Styles[Style.Cpp.Verbatim].ForeColor = string_fore_color; 
		editor.Styles[Style.Cpp.Verbatim].BackColor = string_back_color; 
//        editor.Styles[Style.Cpp.TripleVerbatim].ForeColor = string_fore_color; 
//		editor.Styles[Style.Cpp.TripleVerbatim].BackColor = string_back_color; 
		editor.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
		editor.Styles[Style.Cpp.Operator].ForeColor = operator_color;
		editor.Styles[Style.Cpp.Preprocessor].ForeColor = preprocessor_color;
//        editor.Styles[Style.Cpp.PreprocessorComment].ForeColor = ;
//        editor.Styles[Style.Cpp.PreprocessorCommentDoc].ForeColor = ;
//        editor.Styles[Style.Cpp.UserLiteral].ForeColor = ;
//		editor.Styles[Style.Cpp.TaskMarker].ForeColor = ;
//        editor.Styles[Style.Cpp.EscapeSequence].ForeColor = ;
	}
	public static void set_cs_style(Scintilla editor) { 
		set_c_family_style(editor);
        update_keywords(
            editor,
            "cs",
            "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while",
            "partial bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void var"
        );
	}
	public static void set_cpp_style(Scintilla editor) { 
        set_c_family_style(editor);
        update_keywords(
            editor,
            "cpp",
            "alignas alignof asm auto break case catch class const constexpr const_cast continue decltype default delete do dynamic_cast else enum explicit export extern false for friend goto if inline mutable namespace new noexcept nullptr operator private protected public register reinterpret_cast return sizeof static static_assert static_cast struct switch template this thread_local throw true try typedef typeid typename union using virtual volatile while",
            "bool char char16_t char32_t double float int long short signed unsigned void wchar_t"
        );
    }
    public static void set_c_style(Scintilla editor) { 
        set_c_family_style(editor);
        update_keywords(
            editor,
            "c",
            "auto break case char const continue default do double else enum extern float for goto if inline int long register restrict return short signed sizeof static struct switch typedef union unsigned void volatile while",
            "bool size_t ptrdiff_t"
        );
    }
    public static void set_ahk_style(Scintilla editor) { // TESTING ISSUE
        set_c_family_style(editor);
        update_keywords(
            editor,
            "autohotkey",
            "and byref case const continuecase continueloop default dim do else elseif endfunc endif endselect endswitch enum exit exitloop false for func global if in local next not null or redim return select static step switch then to true until wend while abs acos adlibdisable adlibenable asc asin atan autoitsetoption autoitwingettitle autoitwinsettitle bitand bitnot bitor bitshift bitxor blockinput break call cdtray chr clipget clipput controlclick controlcommand controldisable controlenable controlfocus controlgetfocus controlgetpos controlgettext controlhide controlmove controlsend controlsettext controlshow cos dec dircopy dircreate dirmove dirremove drivegetdrive drivegetfilesystem drivegetlabel drivegetserial drivegettype drivesetlabel drivespacefree drivespacetotal drivestatus envget envset envupdate eval exp filechangedir fileclose filecopy filecreateshortcut filedelete fileexists filefindfirstfile filefindnextfile filegetattrib filegetlongname filegetshortname filegetsize filegettime filegetversion fileinstall filemove fileopen fileopendialog fileread filereadline filerecycle filerecycleempty filesavedialog fileselectfolder filesetattrib filesettime filewrite filewriteline guicreate guicreateex guidefaultfont guidelete guigetcontrolstate guihide guimsg guiread guirecvmsg guisendmsg guisetcontrol guisetcontroldata guisetcontrolex guisetcontrolfont guisetcontrolnotify guisetcoord guisetcursor guishow guiwaitclose guiwrite hex hotkeyset inidelete iniread iniwrite inputbox int isadmin isarray isdeclared isfloat isint isnumber isstring log memgetstats mod mouseclick mouseclickdrag mousedown mousegetcursor mousegetpos mousemove mouseup mousewheel msgbox number pixelchecksum pixelgetcolor pixelsearch processclose processexists processsetpriority processwait processwaitclose progressoff progresson progressset random regdelete regenumkey regenumval regread regwrite round run runasset runwait send seterror shutdown sin sleep soundplay soundsetwavevolume splashimageon splashoff splashtexton sqrt statusbargettext string stringaddcr stringformat stringinstr stringisalnum stringisalpha stringisascii stringisdigit stringisfloat stringisint stringislower stringisspace stringisupper stringisxdigit stringleft stringlen stringlower stringmid stringreplace stringright stringsplit stringstripcr stringstripws stringtrimleft stringtrimright stringupper tan timerstart timerstop tooltip traytip ubound urldownloadtofile winactivate winactive winclose winexists wingetcaretpos wingetclasslist wingetclientsize wingethandle wingetpos wingetstate wingettext wingettitle winkill winmenuselectitem winminimizeall winminimizeallundo winmove winsetontop winsetstate winsettitle winwait winwaitactive winwaitclose winwaitnotactive",
            "{!} {#} {^} {{} {}} {+} {alt} {altdown} {altup} {appskey} {asc} {backspace} {browser_back} {browser_favorites} {browser_forward} {browser_home} {browser_refresh} {browser_search} {browser_stop} {bs} {capslock} {ctrlbreak} {ctrldown} {ctrlup} {del} {delete} {down} {end} {enter} {esc} {escape} {f1} {f10} {f11} {f12} {f2} {f3} {f4} {f5} {f6} {f7} {f8} {f9} {home} {ins} {insert} {lalt} {launch_app1} {launch_app2} {launch_mail} {launch_media} {lctrl} {left} {lshift} {lwin} {lwindown} {media_next} {media_play_pause} {media_prev} {media_stop} {numlock} {numpad0} {numpad1} {numpad2} {numpad3} {numpad4} {numpad5} {numpad6} {numpad7} {numpad8} {numpad9} {numpadadd} {numpaddiv} {numpaddot} {numpadenter} {numpadmult} {numpadsub} {pause} {pgdn} {pgup} {printscreen} {ralt} {rctrl} {right} {rshift} {rwin} {rwindown} {scrolllock} {shiftdown} {shiftup} {sleep} {space} {tab} {up} {volume_down} {volume_mute} {volume_up}"
        );
        post_styling_comment_line(editor, ";");
    }
    public static void set_bash_common_style(Scintilla editor) { 
        editor.Styles[0].ForeColor = default_word_color;
        editor.Styles[2].ForeColor = comment_fore_color;
        editor.Styles[2].BackColor = comment_back_color;
        editor.Styles[3].ForeColor = number_fore_color;
        editor.Styles[3].BackColor = number_back_color;
        editor.Styles[4].ForeColor = keyword1_color;
        editor.Styles[5].ForeColor = string_fore_color;
        editor.Styles[5].BackColor = string_back_color;
        editor.Styles[6].ForeColor = string_fore_color;
        editor.Styles[6].BackColor = string_back_color;
        editor.Styles[7].ForeColor = operator_color;
        // Identifiers
        editor.Styles[8].ForeColor = Color.LightBlue;
        // Variables ($var)
        editor.Styles[9].ForeColor = Color.Orange;
    }
    public static void set_batch_style(Scintilla editor) { 
        set_bash_common_style(editor);
        update_keywords(
            editor,
            "bat",
            "assoc aux break call cd chdir cls cmdextversion color com com1 com2 com3 com4 con copy country ctty date defined del dir do dpath echo else endlocal erase errorlevel exist exit for ftype goto if in loadfix loadhigh lpt lpt1 lpt2 lpt3 lpt4 md mkdir move not nul path pause popd prn prompt pushd rd rem ren rename rmdir set setlocal shift start time title type ver verify vol",
            "taskkill sfc ipconfig netsh start REM if then else elif fi for while do done case esac function select in time until doskey"
        );
    }
    public static void set_ada_style(Scintilla editor) {
        editor.Styles[Style.Ada.Default].ForeColor = default_word_color;
        editor.Styles[Style.Ada.CommentLine].ForeColor = comment_fore_color;
		editor.Styles[Style.Ada.CommentLine].BackColor = comment_back_color;
        editor.Styles[Style.Ada.Number].ForeColor = number_fore_color;
		editor.Styles[Style.Ada.Number].BackColor = number_back_color;
        editor.Styles[Style.Ada.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Ada.String].ForeColor = string_fore_color;
		editor.Styles[Style.Ada.String].BackColor = string_back_color;
        editor.Styles[Style.Ada.Character].ForeColor = string_fore_color;
		editor.Styles[Style.Ada.Character].BackColor = string_back_color;
        editor.Styles[Style.Ada.StringEol].BackColor = Color.Pink;
        editor.Styles[Style.Ada.CharacterEol].BackColor = Color.Pink;
//        editor.Styles[Style.Ada.Delimiter].BackColor = ;
//        editor.Styles[Style.Ada.Label].BackColor = ;
//        editor.Styles[Style.Ada.Identifier].BackColor = ;
//        editor.Styles[Style.Ada.Illegal].BackColor = ;
        update_keywords(
            editor,
            "ada",
            "abort else new	return abs elsif not reverse abstract end null accept entry select access exception of separate	aliased exit or	some all others	subtype and	for	out	synchronized array function overriding at tagged generic package task begin	goto parallel terminate	body pragma	then if	private	type case in procedure constant	interface protected until is use declare raise delay limited range when delta loop record while digits rem with do mod renames requeue xor",
            ""
        );
    }
    public static void set_asm_style(Scintilla editor) { // INCOMPLETE
        editor.Styles[Style.Asm.Default].ForeColor = default_word_color;
        editor.Styles[Style.Asm.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.Asm.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Asm.CommentBlock].ForeColor = comment_fore_color;
        editor.Styles[Style.Asm.CommentBlock].BackColor = comment_back_color;
        editor.Styles[Style.Asm.Number].ForeColor = number_fore_color;
		editor.Styles[Style.Asm.Number].BackColor = number_back_color;
//        editor.Styles[Style.Asm.MathInstruction].BackColor = ;
//        editor.Styles[Style.Asm.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Asm.CpuInstruction].ForeColor = keyword1_color; // 0
        editor.Styles[Style.Asm.MathInstruction].ForeColor = keyword2_color; // 1
        editor.Styles[Style.Asm.Register].ForeColor = keyword2_color; // 2
        editor.Styles[Style.Asm.String].ForeColor = string_fore_color;
		editor.Styles[Style.Asm.String].BackColor = string_back_color;
        editor.Styles[Style.Asm.Character].ForeColor = string_fore_color;
		editor.Styles[Style.Asm.Character].BackColor = string_back_color;
        editor.Styles[Style.Asm.StringEol].BackColor = Color.Pink;
//        editor.Styles[Style.Asm.CharacterEol].BackColor = Color.Pink;
        editor.Styles[Style.Asm.Operator].ForeColor = operator_color;
        editor.Styles[Style.Asm.Directive].ForeColor = operator_color;
        editor.Styles[Style.Asm.DirectiveOperand].ForeColor = operator_color;
        editor.Styles[Style.Asm.CommentDirective].ForeColor = preprocessor_color;
        editor.Styles[Style.Asm.ExtInstruction].ForeColor = operator_color;
//        editor.Styles[Style.Asm.Identifier].ForeColor = ;
        update_keywords(
            editor,
            "asm",
            "",
            ""
        );
    }
    public static void set_blitzbasic_style(Scintilla editor) { // ISSUE - autofolding don't work
        editor.Styles[Style.BlitzBasic.Default].ForeColor = default_word_color;
        editor.Styles[Style.BlitzBasic.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.BlitzBasic.Comment].BackColor = comment_back_color;
        editor.Styles[Style.BlitzBasic.Number].ForeColor = number_fore_color;
		editor.Styles[Style.BlitzBasic.Number].BackColor = number_back_color;
        editor.Styles[Style.BlitzBasic.Keyword].ForeColor = keyword1_color;
        editor.Styles[Style.BlitzBasic.String].ForeColor = string_fore_color;
		editor.Styles[Style.BlitzBasic.String].BackColor = string_back_color;
        editor.Styles[Style.BlitzBasic.Preprocessor].ForeColor = preprocessor_color;
        editor.Styles[Style.BlitzBasic.Operator].ForeColor = operator_color;
//        editor.Styles[Style.BlitzBasic.Identifier].BackColor = ;
//        editor.Styles[Style.BlitzBasic.Date].ForeColor = ;
        editor.Styles[Style.BlitzBasic.StringEol].BackColor = Color.Pink;
        editor.Styles[Style.BlitzBasic.Keyword2].ForeColor = keyword2_color;
        editor.Styles[Style.BlitzBasic.Keyword3].ForeColor = keyword2_color;
        editor.Styles[Style.BlitzBasic.Keyword4].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Constant].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Asm].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Label].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Error].ForeColor = keyword2_color;
        editor.Styles[Style.BlitzBasic.HexNumber].ForeColor = number_fore_color;
        editor.Styles[Style.BlitzBasic.HexNumber].BackColor = number_back_color;
        editor.Styles[Style.BlitzBasic.BinNumber].ForeColor = number_fore_color;
        editor.Styles[Style.BlitzBasic.BinNumber].BackColor = number_back_color;
        editor.Styles[Style.BlitzBasic.CommentBlock].ForeColor = comment_fore_color;
        editor.Styles[Style.BlitzBasic.CommentBlock].BackColor = comment_back_color;
        editor.Styles[Style.BlitzBasic.DocLine].ForeColor = comment_fore_color;
        editor.Styles[Style.BlitzBasic.DocLine].BackColor = comment_back_color;
        editor.Styles[Style.BlitzBasic.DocBlock].ForeColor = comment_fore_color;
        editor.Styles[Style.BlitzBasic.DocBlock].BackColor = comment_back_color;
        editor.Styles[Style.BlitzBasic.DocKeyword].ForeColor = Color.White;
//        editor.Styles[Style.BlitzBasic.CommentLine].ForeColor = comment_fore_color;
//		editor.Styles[Style.BlitzBasic.CommentLine].BackColor = comment_back_color;
//        editor.Styles[Style.BlitzBasic.Word].ForeColor = keyword1_color;
        update_keywords(
            editor,
            "blitzbasic",
            "abs accepttcpstream acos after and apptitle asc asin atan atan2 automidhandle autosuspend availvidmem backbuffer banksize before bin calldll case ceil changedir channelpan channelpitch channelplaying channelvolume chr closedir closefile closemovie closetcpserver closetcpstream closeudpstream cls clscolor color colorblue colorgreen colorred commandline const copybank copyfile copyimage copypixel copypixelfast copyrect copystream cos countgfxdrivers countgfxmodes counthostips createbank createdir createimage createnetplayer createprocess createtcpserver createtimer createudpstream currentdate currentdir currenttime data debuglog default delay delete deletedir deletefile deletenetplayer desktopbuffer dim dottedip drawblock drawblockrect drawimage drawimagerect drawmovie each else else if elseif end end function end if end select end type endgraphics endif eof execfile exit exp false field filepos filesize filetype first flip float floor flushjoy flushkeys flushmouse fontheight fontname fontsize fontstyle fontwidth for forever freebank freefont freeimage freesound freetimer frontbuffer function gammablue gammagreen gammared getcolor getenv getkey getmouse gfxdrivername gfxmodedepth gfxmodeexists gfxmodeformat gfxmodeheight gfxmodewidth global gosub goto grabimage graphics graphicsbuffer graphicsdepth graphicsformat graphicsheight graphicswidth handleimage hex hidepointer hostip hostnetgame if imagebuffer imageheight imagerectcollide imagerectoverlap imagescollide imagesoverlap imagewidth imagexhandle imageyhandle include input insert instr int joinnetgame joydown joyhat joyhit joypitch joyroll joytype joyu joyudir joyv joyvdir joyx joyxdir joyy joyyaw joyydir joyz joyzdir keydown keyhit keywait last left len line loadanimimage loadbuffer loadfont loadimage loadsound local lockbuffer lockedformat lockedpitch lockedpixels log log10 loopsound lower lset maskimage mid midhandle millisecs mod morefiles mousedown mousehit mousex mousexspeed mousey mouseyspeed mousez mousezspeed movemouse movieheight movieplaying moviewidth netmsgdata netmsgfrom netmsgto netmsgtype netplayerlocal netplayername new next nextfile not null openfile openmovie opentcpstream or origin oval pausechannel pausetimer peekbyte peekfloat peekint peekshort pi playcdtrack playmusic playsound plot pokebyte pokefloat pokeint pokeshort print queryobject rand read readavail readbyte readbytes readdir readfile readfloat readint readline readpixel readpixelfast readshort readstring rect rectsoverlap recvnetmsg recvudpmsg repeat replace resettimer resizebank resizeimage restore resumechannel resumetimer return right rnd rndseed rotateimage rset runtimeerror sar savebuffer saveimage scaleimage scanline seedrnd seekfile select sendnetmsg sendudpmsg setbuffer setenv setfont setgamma setgfxdriver sgn shl showpointer shr sin soundpan soundpitch soundvolume sqr startnetgame step stop stopchannel stopnetgame str string stringheight stringwidth systemproperty tan tcpstreamip tcpstreamport tcptimeouts text tformfilter tformimage then tileblock tileimage timerticks to totalvidmem trim true type udpmsgip udpmsgport udpstreamip udpstreamport udptimeouts unlockbuffer until updategamma upper viewport vwait waitkey waitmouse waittimer wend while write writebyte writebytes writefile writefloat writeint writeline writepixel writepixelfast writeshort writestring xor",
            ""
        );
    }
    public static void set_clw_style(Scintilla editor) { // ISSUE - no clue what is this language ... 
//        editor.Styles[Style.CLW.Attributes].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.BuiltInProceduresFunction].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.Comment].ForeColor = comment_fore_color;
//        editor.Styles[Style.CLW.Comment].BackColor = comment_back_color;
//        editor.Styles[Style.CLW.CompilerDirective].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.Default].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.Depreciated].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.Error].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.IntegerConstant].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.Keyword].ForeColor = keyword1_color;
//        editor.Styles[Style.CLW.Label].ForeColor = string_fore_color;
//        editor.Styles[Style.CLW.Label].BackColor = string_back_color;
//        editor.Styles[Style.CLW.String].ForeColor = string_fore_color;
//        editor.Styles[Style.CLW.String].BackColor = string_back_color;
//        editor.Styles[Style.CLW.PictureString].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.RealConstant].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.RuntimeExpressions].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.StandardEquates].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.StructureDataTypes].ForeColor = default_word_color;
//        editor.Styles[Style.CLW.UserIdentifier].ForeColor = default_word_color;
    }
    public static void set_css_style(Scintilla editor) { // INCOMPLETE - keywords up 2
        editor.Styles[Style.Css.Default].ForeColor = default_word_color;
        editor.Styles[Style.Css.Tag].ForeColor = keyword1_color;
        editor.Styles[Style.Css.Attribute].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Class].ForeColor = keyword2_color;
        editor.Styles[Style.Css.PseudoClass].ForeColor = keyword2_color;
        editor.Styles[Style.Css.UnknownPseudoClass].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Operator].ForeColor = operator_color;
        editor.Styles[Style.Css.Identifier].ForeColor = default_word_color;
        editor.Styles[Style.Css.UnknownIdentifier].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Value].ForeColor = number_fore_color;
        editor.Styles[Style.Css.Value].BackColor = number_back_color;
        editor.Styles[Style.Css.DoubleString].ForeColor = string_fore_color;
        editor.Styles[Style.Css.DoubleString].BackColor = string_back_color;
        editor.Styles[Style.Css.SingleString].ForeColor = string_fore_color;
        editor.Styles[Style.Css.SingleString].BackColor = string_back_color;
        editor.Styles[Style.Css.Id].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Important].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Directive].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Identifier2].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Identifier3].ForeColor = keyword2_color;
        editor.Styles[Style.Css.PseudoElement].ForeColor = keyword2_color;
        editor.Styles[Style.Css.ExtendedIdentifier].ForeColor = keyword2_color;
        editor.Styles[Style.Css.ExtendedPseudoClass].ForeColor = keyword2_color;
        editor.Styles[Style.Css.ExtendedPseudoElement].ForeColor = keyword2_color;
        editor.Styles[Style.Css.Media].ForeColor = string_fore_color;
        editor.Styles[Style.Css.Media].BackColor = string_back_color;
        editor.Styles[Style.Css.Variable].ForeColor = keyword2_color;
//        editor.Styles[Style.Css.Number].ForeColor = number_fore_color;
//        editor.Styles[Style.Css.Number].BackColor = number_back_color;
//        editor.Styles[Style.Css.Other].ForeColor = string_fore_color;
//        editor.Styles[Style.Css.Other].BackColor = string_back_color;
        editor.Styles[Style.Css.Comment].ForeColor = comment_fore_color;
        editor.Styles[Style.Css.Comment].BackColor = comment_back_color;
//        editor.Styles[Style.Css.Entity].ForeColor = Color.LightGreen;
        update_keywords(editor,"css",
            "-khtml-background-clip -khtml-background-origin -khtml-background-size -khtml-border-bottom-left-radius -khtml-border-bottom-right-radius -khtml-border-radius -khtml-border-top-left-radius -khtml-border-top-right-radius -khtml-opacity -moz-animation -moz-animation-delay -moz-animation-direction -moz-animation-duration -moz-animation-fill-mode -moz-animation-iteration-count -moz-animation-name -moz-animation-play-state -moz-animation-timing-function -moz-appearance -moz-background-clip -moz-background-inline-policy -moz-background-origin -moz-background-size -moz-binding -moz-border-bottom-colors -moz-border-end -moz-border-end-color -moz-border-end-style -moz-border-end-width -moz-border-image -moz-border-left-colors -moz-border-radius -moz-border-radius-bottomleft -moz-border-radius-bottomright -moz-border-radius-topleft -moz-border-radius-topright -moz-border-right-colors -moz-border-start -moz-border-start-color -moz-border-start-style -moz-border-start-width -moz-border-top-colors -moz-box-align -moz-box-direction -moz-box-flex -moz-box-flex-group -moz-box-flexgroup -moz-box-ordinal-group -moz-box-orient -moz-box-pack -moz-box-shadow -moz-box-sizing -moz-column-count -moz-column-gap -moz-column-rule -moz-column-rule-color -moz-column-rule-style -moz-column-rule-width -moz-column-width -moz-context-properties -moz-float-edge -moz-force-broken-image-icon -moz-image-region -moz-linear-gradient -moz-margin-end -moz-margin-start -moz-opacity -moz-outline -moz-outline-color -moz-outline-offset -moz-outline-radius -moz-outline-radius-bottomleft -moz-outline-radius-bottomright -moz-outline-radius-topleft -moz-outline-radius-topright -moz-outline-style -moz-outline-width -moz-padding-end -moz-padding-start -moz-radial-gradient -moz-stack-sizing -moz-text-decoration-color -moz-text-decoration-line -moz-text-decoration-style -moz-transform -moz-transform-origin -moz-transition -moz-transition-delay -moz-transition-duration -moz-transition-property -moz-transition-timing-function -moz-user-focus -moz-user-input -moz-user-modify -moz-user-select -moz-window-shadow -ms-filter -ms-transform -ms-transform-origin -o-transform -webkit-animation -webkit-animation-delay -webkit-animation-direction -webkit-animation-duration -webkit-animation-fill-mode -webkit-animation-iteration-count -webkit-animation-name -webkit-animation-play-state -webkit-animation-timing-function -webkit-appearance -webkit-backface-visibility -webkit-background-clip -webkit-background-composite -webkit-background-origin -webkit-background-size -webkit-border-bottom-left-radius -webkit-border-bottom-right-radius -webkit-border-horizontal-spacing -webkit-border-image -webkit-border-radius -webkit-border-top-left-radius -webkit-border-top-right-radius -webkit-border-vertical-spacing -webkit-box-align -webkit-box-direction -webkit-box-flex -webkit-box-flex-group -webkit-box-lines -webkit-box-ordinal-group -webkit-box-orient -webkit-box-pack -webkit-box-reflect -webkit-box-shadow -webkit-box-sizing -webkit-column-break-after -webkit-column-break-before -webkit-column-break-inside -webkit-column-count -webkit-column-gap -webkit-column-rule -webkit-column-rule-color -webkit-column-rule-style -webkit-column-rule-width -webkit-column-width -webkit-columns -webkit-dashboard-region -webkit-font-smoothing -webkit-gradient -webkit-line-break -webkit-linear-gradient -webkit-margin-bottom-collapse -webkit-margin-collapse -webkit-margin-start -webkit-margin-top-collapse -webkit-marquee -webkit-marquee-direction -webkit-marquee-increment -webkit-marquee-repetition -webkit-marquee-speed -webkit-marquee-style -webkit-mask -webkit-mask-attachment -webkit-mask-box-image -webkit-mask-clip -webkit-mask-composite -webkit-mask-image -webkit-mask-origin -webkit-mask-position -webkit-mask-position-x -webkit-mask-position-y -webkit-mask-repeat -webkit-mask-size -webkit-nbsp-mode -webkit-padding-start -webkit-perspective -webkit-perspective-origin -webkit-radial-gradient -webkit-rtl-ordering -webkit-tap-highlight-color -webkit-text-fill-color -webkit-text-security -webkit-text-size-adjust -webkit-text-stroke -webkit-text-stroke-color -webkit-text-stroke-width -webkit-touch-callout -webkit-transform -webkit-transform-origin -webkit-transform-origin-x -webkit-transform-origin-y -webkit-transform-origin-z -webkit-transform-style -webkit-transition -webkit-transition-delay -webkit-transition-duration -webkit-transition-property -webkit-transition-timing-function -webkit-user-drag -webkit-user-modify -webkit-user-select align-content align-items align-self alignment-adjust alignment-baseline all animation animation-delay animation-direction animation-duration animation-fill-mode animation-iteration-count animation-name animation-play-state animation-timing-function appearance azimuth backface-visibility background background-attachment background-blend-mode background-break background-clip background-color background-image background-origin background-position background-position-x background-position-y background-repeat background-size baseline-shift binding bleed block-size bookmark-label bookmark-level bookmark-state bookmark-target border border-block border-block-end border-block-start border-bottom border-bottom-color border-bottom-left-radius border-bottom-right-radius border-bottom-style border-bottom-width border-collapse border-color border-image border-image-outset border-image-repeat border-image-slice border-image-source border-image-width border-inline border-inline-end border-inline-end-width border-inline-start border-inline-start-color border-inline-start-style border-inline-start-width border-left border-left-color border-left-style border-left-width border-radius border-right border-right-color border-right-style border-right-width border-spacing border-style border-top border-top-color border-top-left-radius border-top-right-radius border-top-style border-top-width border-width bottom box-align box-decoration-break box-direction box-flex box-flex-group box-lines box-ordinal-group box-orient box-pack box-shadow box-sizing break-after break-before break-inside caption-side caret-color clear clip color color-profile color-scheme column-count column-fill column-gap column-rule column-rule-color column-rule-style column-rule-width column-span column-width columns content counter-increment counter-reset crop cue cue-after cue-before cursor direction display dominant-baseline drop-initial-after-adjust drop-initial-after-align drop-initial-before-adjust drop-initial-before-align drop-initial-size drop-initial-value elevation empty-cells field-sizing fill fill-opacity filter fit fit-position flex flex-basis flex-direction flex-flow flex-grow flex-shrink flex-wrap float float-offset font font-effect font-emphasize font-family font-size font-size-adjust font-stretch font-style font-variant font-variant-ligatures font-weight gap grid-area grid-auto-flow grid-auto-rows grid-column grid-column-end grid-column-gap grid-column-start grid-columns grid-gap grid-row grid-row-gap grid-rows grid-template-areas grid-template-columns grid-template-rows hanging-punctuation height hyphenate-after hyphenate-before hyphenate-character hyphenate-lines hyphenate-resource hyphens icon image-orientation image-rendering image-resolution inline-box-align inline-size inset inset-inline-end inset-inline-start justify-content justify-items justify-self left letter-spacing line-height line-stacking line-stacking-ruby line-stacking-shift line-stacking-strategy list-style list-style-image list-style-position list-style-type margin margin-block margin-block-end margin-block-start margin-bottom margin-inline margin-inline-end margin-inline-start margin-left margin-right margin-top mark mark-after mark-before marker-offset marks marquee-direction marquee-play-count marquee-speed marquee-style mask mask-clip mask-image mask-origin mask-position mask-position-x mask-repeat mask-size max-height max-width min-block-size min-height min-inline-size min-width mix-blend-mode move-to nav-down nav-index nav-left nav-right nav-up object-fit opacity order orphans outline outline-color outline-offset outline-style outline-width overflow overflow-anchor overflow-style overflow-wrap overflow-x overflow-y padding padding-block padding-block-end padding-block-start padding-bottom padding-inline padding-inline-end padding-inline-start padding-left padding-right padding-top page page-break-after page-break-before page-break-inside page-policy paint-order pause pause-after pause-before perspective perspective-origin phonemes pitch pitch-range play-during pointer-events position presentation-level punctuation-trim quotes rendering-intent resize rest rest-after rest-before richness right rotation rotation-point ruby-align ruby-overhang ruby-position ruby-span scrollbar-color size speak speak-header speak-numeral speak-punctuation speech-rate stress string-set stroke stroke-dasharray stroke-linejoin stroke-opacity stroke-width tab-size table-layout target target-name target-new target-position text-align text-align-last text-anchor text-decoration text-decoration-color text-decoration-line text-decoration-style text-emphasis text-height text-indent text-justify text-outline text-overflow text-rendering text-shadow text-transform text-wrap top transform transform-origin transform-style transition transition-delay transition-duration transition-property transition-timing-function unicode-bidi user-select vector-effect vertical-align visibility voice-balance voice-duration voice-family voice-pitch voice-pitch-range voice-rate voice-stress voice-volume volume white-space white-space-collapse widows width will-change word-break word-spacing word-wrap z-index",
            "active any-link autofill checked default defined disabled empty enabled first first-child first-of-type focus focus-visible focus-within fullscreen has host hover in-range indeterminate invalid is lang last-child last-of-type left link modal not nth-child nth-last-child nth-last-of-type nth-of-type only-child only-of-type optional out-of-range placeholder-shown read-only read-write required right root scope target valid visited where"
        );
    }
    public static void set_fortran_style(Scintilla editor) { 
        editor.Styles[Style.Fortran.Default].ForeColor = default_word_color;
		editor.Styles[Style.Fortran.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.Fortran.Comment].BackColor = comment_back_color;
		editor.Styles[Style.Fortran.Number].ForeColor = number_fore_color;
		editor.Styles[Style.Fortran.Number].BackColor = number_back_color;
        editor.Styles[Style.Fortran.Word].ForeColor = keyword1_color;
		editor.Styles[Style.Fortran.Word2].ForeColor = keyword2_color;
        editor.Styles[Style.Fortran.Word3].ForeColor = keyword2_color;
		editor.Styles[Style.Fortran.String1].ForeColor = string_fore_color;
		editor.Styles[Style.Fortran.String1].BackColor = string_back_color;
        editor.Styles[Style.Fortran.String2].ForeColor = string_fore_color;
		editor.Styles[Style.Fortran.String2].BackColor = string_back_color;
        editor.Styles[Style.Fortran.Label].ForeColor = string_fore_color;
		editor.Styles[Style.Fortran.Label].BackColor = string_back_color;
//		editor.Styles[Style.Fortran.Character].ForeColor = string_fore_color;
//		editor.Styles[Style.Fortran.Character].BackColor = string_back_color;
//		editor.Styles[Style.Fortran.Verbatim].ForeColor = string_fore_color; 
//		editor.Styles[Style.Fortran.Verbatim].BackColor = string_back_color; 
		editor.Styles[Style.Fortran.StringEol].BackColor = Color.Pink;
		editor.Styles[Style.Fortran.Operator].ForeColor = operator_color;
        editor.Styles[Style.Fortran.Operator2].ForeColor = operator_color;
		editor.Styles[Style.Fortran.Preprocessor].ForeColor = preprocessor_color;
        editor.Styles[Style.Fortran.Continuation].ForeColor = default_word_color;
        update_keywords(editor,"fortran",
            "access action advance allocatable allocate apostrophe assign assignment associate asynchronous backspace bind blank blockdata call case character class close common complex contains continue critical cycle data deallocate decimal delim default dimension direct do dowhile double doubleprecision else elseif elsewhere encoding end endassociate endblockdata endcritical enddo endenum endfile endforall endfunction endif endinterface endmodule endprocedure endprogram endselect endsubmodule endsubroutine endtype endwhere entry enum eor equivalence err errmsg exist exit external file flush fmt forall form format formatted function go goto id if implicit in include inout integer inquire intent interface intrinsic iomsg iolength iostat kind len logical module name named namelist nextrec nml none nullify number only open opened operator optional out pad parameter pass pause pending pointer pos position precision print private procedure program protected public quote read readwrite real rec recl recursive result return rewind save select selectcase selecttype sequential sign size stat status stop stream submodule subroutine target then to type unformatted unit use value volatile wait where while write",
            "abs achar acos acosd adjustl adjustr aimag aimax0 aimin0 aint ajmax0 ajmin0 akmax0 akmin0 all allocated alog alog10 amax0 amax1 amin0 amin1 amod anint any asin asind associated atan atan2 atan2d atand bitest bitl bitlr bitrl bjtest bit_size bktest break btest cabs ccos cdabs cdcos cdexp cdlog cdsin cdsqrt ceiling cexp char clog cmplx conjg cos cosd cosh count cpu_time cshift csin csqrt dabs dacos dacosd dasin dasind datan datan2 datan2d datand date date_and_time dble dcmplx dconjg dcos dcosd dcosh dcotan ddim dexp dfloat dflotk dfloti dflotj digits dim dimag dint dlog dlog10 dmax1 dmin1 dmod dnint dot_product dprod dreal dsign dsin dsind dsinh dsqrt dtan dtand dtanh eoshift epsilon errsns exp exponent float floati floatj floatk floor fraction free huge iabs iachar iand ibclr ibits ibset ichar idate idim idint idnint ieor ifix iiabs iiand iibclr iibits iibset iidim iidint iidnnt iieor iifix iint iior iiqint iiqnnt iishft iishftc iisign ilen imax0 imax1 imin0 imin1 imod index inint inot int int1 int2 int4 int8 iqint iqnint ior ishft ishftc isign isnan izext jiand jibclr jibits jibset jidim jidint jidnnt jieor jifix jint jior jiqint jiqnnt jishft jishftc jisign jmax0 jmax1 jmin0 jmin1 jmod jnint jnot jzext kiabs kiand kibclr kibits kibset kidim kidint kidnnt kieor kifix kind kint kior kishft kishftc kisign kmax0 kmax1 kmin0 kmin1 kmod knint knot kzext lbound leadz len len_trim lenlge lge lgt lle llt log log10 logical lshift malloc matmul max max0 max1 maxexponent maxloc maxval merge min min0 min1 minexponent minloc minval mod modulo mvbits nearest nint not nworkers number_of_processors pack popcnt poppar precision present product radix random random_number random_seed range real repeat reshape rrspacing rshift scale scan secnds selected_int_kind selected_real_kind set_exponent shape sign sin sind sinh size sizeof sngl snglq spacing spread sqrt sum system_clock tan tand tanh tiny transfer transpose trim ubound unpack verify",
            "cdabs cdcos cdexp cdlog cdsin cdsqrt cotan cotand dcmplx dconjg dcotan dcotand decode dimag dll_export dll_import doublecomplex dreal dvchk encode find flen flush getarg getcharqq getcl getdat getenv gettim hfix ibchng identifier imag int1 int2 int4 intc intrup invalop iostat_msg isha ishc ishl jfix lacfar locking locnear map nargs nbreak ndperr ndpexc offset ovefl peekcharqq precfill prompt qabs qacos qacosd qasin qasind qatan qatand qatan2 qcmplx qconjg qcos qcosd qcosh qdim qexp qext qextd qfloat qimag qlog qlog10 qmax1 qmin1 qmod qreal qsign qsin qsind qsinh qsqrt qtan qtand qtanh ran rand randu rewrite segment setdat settim system timer undfl unlock union val virtual volatile zabs zcos zexp zlog zsin zsqrt"
        );
    }
    public static void set_freebasic_style(Scintilla editor) {
        editor.Styles[Style.FreeBasic.Default].ForeColor = default_word_color;
        editor.Styles[Style.FreeBasic.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.FreeBasic.Comment].BackColor = comment_back_color;
        editor.Styles[Style.FreeBasic.Number].ForeColor = number_fore_color;
		editor.Styles[Style.FreeBasic.Number].BackColor = number_back_color;
        editor.Styles[Style.FreeBasic.Keyword].ForeColor = keyword1_color;
        editor.Styles[Style.FreeBasic.String].ForeColor = string_fore_color;
		editor.Styles[Style.FreeBasic.String].BackColor = string_back_color;
        editor.Styles[Style.FreeBasic.Preprocessor].ForeColor = preprocessor_color;
        editor.Styles[Style.FreeBasic.Operator].ForeColor = operator_color;
//        editor.Styles[Style.BlitzBasic.Identifier].BackColor = ;
//        editor.Styles[Style.BlitzBasic.Date].ForeColor = ;
        editor.Styles[Style.FreeBasic.StringEol].BackColor = Color.Pink;
        editor.Styles[Style.FreeBasic.Keyword2].ForeColor = keyword2_color;
        editor.Styles[Style.FreeBasic.Keyword3].ForeColor = keyword2_color;
        editor.Styles[Style.FreeBasic.Keyword4].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Constant].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Asm].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Label].ForeColor = keyword2_color;
//        editor.Styles[Style.BlitzBasic.Error].ForeColor = keyword2_color;
        editor.Styles[Style.FreeBasic.HexNumber].ForeColor = number_fore_color;
        editor.Styles[Style.FreeBasic.HexNumber].BackColor = number_back_color;
        editor.Styles[Style.FreeBasic.BinNumber].ForeColor = number_fore_color;
        editor.Styles[Style.FreeBasic.BinNumber].BackColor = number_back_color;
        editor.Styles[Style.FreeBasic.CommentBlock].ForeColor = comment_fore_color;
        editor.Styles[Style.FreeBasic.CommentBlock].BackColor = comment_back_color;
        editor.Styles[Style.FreeBasic.DocLine].ForeColor = comment_fore_color;
        editor.Styles[Style.FreeBasic.DocLine].BackColor = comment_back_color;
        editor.Styles[Style.FreeBasic.DocBlock].ForeColor = comment_fore_color;
        editor.Styles[Style.FreeBasic.DocBlock].BackColor = comment_back_color;
        editor.Styles[Style.FreeBasic.DocKeyword].ForeColor = Color.White;
//        editor.Styles[Style.BlitzBasic.CommentLine].ForeColor = comment_fore_color;
//		editor.Styles[Style.BlitzBasic.CommentLine].BackColor = comment_back_color;
//        editor.Styles[Style.BlitzBasic.Word].ForeColor = keyword1_color;
        update_keywords(editor, "freebasic", 
            "append as asc asin asm atan2 atn beep bin binary bit bitreset bitset bload bsave byref byte byval call callocate case cbyte cdbl cdecl chain chdir chr cint circle clear clng clngint close cls color command common cons const continue cos cshort csign csng csrlin cubyte cuint culngint cunsg curdir cushort custom cvd cvi cvl cvlongint cvs cvshort data date deallocate declare defbyte defdbl defined defint deflng deflngint defshort defsng defstr defubyte defuint defulngint defushort dim dir do double draw dylibload dylibsymbol else elseif end enum environ environ$ eof eqv erase err error exec exepath exit exp export extern field fix flip for fre freefile function get getjoystick getkey getmouse gosub goto hex hibyte hiword if iif imagecreate imagedestroy imp inkey inp input instr int integer is kill lbound lcase left len let lib line lobyte loc local locate lock lof log long longint loop loword lset ltrim mid mkd mkdir mki mkl mklongint mks mkshort mod multikey mutexcreate mutexdestroy mutexlock mutexunlock name next not oct on once open option or out output overload paint palette pascal pcopy peek peeki peeks pipe pmap point pointer poke pokei pokes pos preserve preset print private procptr pset ptr public put random randomize read reallocate redim rem reset restore resume resume next return rgb rgba right rmdir rnd rset rtrim run sadd screen screencopy screeninfo screenlock screenptr screenres screenset screensync screenunlock seek statement seek function selectcase setdate setenviron setmouse settime sgn shared shell shl short shr sin single sizeof sleep space spc sqr static stdcall step stop str string string strptr sub swap system tab tan then threadcreate threadwait time time timer to trans trim type ubound ubyte ucase uinteger ulongint union unlock unsigned until ushort using va_arg va_first va_next val val64 valint varptr view viewprint wait wend while width window windowtitle with write xor zstring",
            "#define #dynamic #else #endif #error #if #ifdef #ifndef #inclib #include #print #static #undef"
        );
    }
    public static void set_json_style(Scintilla editor) {
        editor.Styles[Style.Json.Default].ForeColor = default_word_color;
        editor.Styles[Style.Json.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Json.Number].BackColor = number_back_color;
        editor.Styles[Style.Json.String].ForeColor = string_fore_color;
        editor.Styles[Style.Json.String].BackColor = string_back_color;
        editor.Styles[Style.Json.StringEol].BackColor = Color.Pink;
        editor.Styles[Style.Json.PropertyName].ForeColor = default_word_color;
        editor.Styles[Style.Json.EscapeSequence].ForeColor = default_word_color;
        editor.Styles[Style.Json.LineComment].ForeColor = comment_fore_color;
        editor.Styles[Style.Json.LineComment].BackColor = comment_back_color;
        editor.Styles[Style.Json.BlockComment].ForeColor = comment_fore_color;
        editor.Styles[Style.Json.BlockComment].BackColor = comment_back_color;
        editor.Styles[Style.Json.Operator].ForeColor = operator_color;
        editor.Styles[Style.Json.Uri].ForeColor = string_fore_color;
        editor.Styles[Style.Json.Uri].BackColor = string_back_color;
        editor.Styles[Style.Json.CompactIRI].ForeColor = string_fore_color;
        editor.Styles[Style.Json.CompactIRI].BackColor = string_back_color;
        editor.Styles[Style.Json.Keyword].ForeColor = keyword1_color;
        editor.Styles[Style.Json.LdKeyword].ForeColor = keyword2_color;
        editor.Styles[Style.Json.Error].ForeColor = preprocessor_color;
        update_keywords(editor,"json",
            "false null true",
            "@id @context @type @value @language @container @list @set @reverse @index @base @vocab @graph"
        );
    }
    public static void set_lisp_style(Scintilla editor) {
        editor.Styles[Style.Lisp.Default].ForeColor = default_word_color;
		editor.Styles[Style.Lisp.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.Lisp.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Lisp.Number].ForeColor = number_fore_color;
		editor.Styles[Style.Lisp.Number].BackColor = number_back_color;
        editor.Styles[Style.Lisp.Keyword].ForeColor = keyword1_color;
        editor.Styles[Style.Lisp.KeywordKw].ForeColor = keyword2_color;
        editor.Styles[Style.Lisp.Symbol].ForeColor = operator_color;
        editor.Styles[Style.Lisp.String].ForeColor = string_fore_color;
		editor.Styles[Style.Lisp.String].BackColor = string_back_color;
        editor.Styles[Style.Lisp.StringEol].BackColor = Color.Pink;
        editor.Styles[Style.Lisp.Identifier].ForeColor = default_word_color;
        editor.Styles[Style.Lisp.Operator].ForeColor = operator_color;
        editor.Styles[Style.Lisp.Special].ForeColor = keyword2_color;
        editor.Styles[Style.Lisp.MultiComment].ForeColor = comment_fore_color;
		editor.Styles[Style.Lisp.MultiComment].BackColor = comment_back_color;
        update_keywords(editor,"lisp",
            "not defun + - * / = &lt; &gt; &lt;= &gt;= princ eval apply funcall quote identity function complement backquote lambda set setq setf defmacro gensym make symbol intern name value plist get getf putprop remprop hash array aref car cdr caar cadr cdar cddr caaar caadr cadar caddr cdaar cdadr cddar cdddr caaaar caaadr caadar caaddr cadaar cadadr caddar cadddr cdaaar cdaadr cdadar cdaddr cddaar cddadr cdddar cddddr cons list append reverse last nth nthcdr member assoc subst sublis nsubst nsublis remove length mapc mapcar mapl maplist mapcan mapcon rplaca rplacd nconc delete atom symbolp numberp boundp null listp consp minusp zerop plusp evenp oddp eq eql equal cond case and or let l if prog prog1 prog2 progn go return do dolist dotimes catch throw error cerror break continue errset baktrace evalhook truncate float rem min max abs sin cos tan expt exp sqrt random logand logior logxor lognot bignums logeqv lognand lognor logorc2 logtest logbitp logcount integer nil",
            ""
        );
    }
    public static void set_matlab_style(Scintilla editor) {
        editor.Styles[Style.Matlab.Default].ForeColor = default_word_color;
		editor.Styles[Style.Matlab.Comment].ForeColor = comment_fore_color;
		editor.Styles[Style.Matlab.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Matlab.Number].ForeColor = number_fore_color;
		editor.Styles[Style.Matlab.Number].BackColor = number_back_color;
        editor.Styles[Style.Matlab.String].ForeColor = string_fore_color;
		editor.Styles[Style.Matlab.String].BackColor = string_back_color;
        editor.Styles[Style.Matlab.Command].ForeColor = operator_color;
        editor.Styles[Style.Matlab.Keyword].ForeColor = keyword1_color;
        editor.Styles[Style.Matlab.DoubleQuoteString].ForeColor = string_fore_color;
		editor.Styles[Style.Matlab.DoubleQuoteString].BackColor = string_back_color;        
        editor.Styles[Style.Matlab.Identifier].ForeColor = default_word_color;
        editor.Styles[Style.Matlab.Operator].ForeColor = operator_color;
        update_keywords(editor,"matlab",
            "break case catch classdef continue else elseif end for function global if otherwise parfor persistent return switch try while",
            ""
        );
    }
    public static void set_rust_style(Scintilla editor) { // ISSUE - c family mess all the styling
        set_c_family_style(editor);
        update_keywords(editor,"rust",
            "abstract as async await become box break const continue crate do dyn else enum extern false final fn for gen if impl in let loop macro macro_rules match mod move mut override priv pub raw ref return safe self static struct super trait true try type typeof union unsafe unsized use virtual where while yield",
            "bool char f32 f64 i128 i16 i32 i64 i8 isize str u128 u16 u32 u64 u8 usize"
        );
    }
    public static void set_perl_style(Scintilla editor) {
        editor.Styles[Style.Perl.Default].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Error].ForeColor = default_word_color;
        editor.Styles[Style.Perl.CommentLine].ForeColor = comment_fore_color;
        editor.Styles[Style.Perl.CommentLine].BackColor = comment_back_color;
        editor.Styles[Style.Perl.Pod].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Perl.Number].BackColor = number_back_color;
        editor.Styles[Style.Perl.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Perl.String].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.String].BackColor = string_back_color;
        editor.Styles[Style.Perl.Character].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.Character].BackColor = string_back_color;
        editor.Styles[Style.Perl.Punctuation].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Preprocessor].ForeColor = preprocessor_color;
        editor.Styles[Style.Perl.Operator].ForeColor = operator_color;
        editor.Styles[Style.Perl.Identifier].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Scalar].ForeColor = number_fore_color;
        editor.Styles[Style.Perl.Scalar].BackColor = number_back_color;
        editor.Styles[Style.Perl.Array].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Hash].ForeColor = number_fore_color;
        editor.Styles[Style.Perl.Hash].BackColor = number_back_color;
        editor.Styles[Style.Perl.SymbolTable].ForeColor = default_word_color;
        editor.Styles[Style.Perl.VariableIndexer].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Regex].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.Regex].BackColor = string_back_color;
        editor.Styles[Style.Perl.RegSubst].ForeColor = default_word_color;
        editor.Styles[Style.Perl.BackTicks].ForeColor = default_word_color;
        editor.Styles[Style.Perl.DataSection].ForeColor = default_word_color;
        editor.Styles[Style.Perl.HereDelim].ForeColor = default_word_color;        
        editor.Styles[Style.Perl.HereQ].ForeColor = default_word_color;        
        editor.Styles[Style.Perl.HereQq].ForeColor = default_word_color;        
        editor.Styles[Style.Perl.HereQx].ForeColor = default_word_color;                
        editor.Styles[Style.Perl.StringQ].ForeColor = string_fore_color;        
        editor.Styles[Style.Perl.StringQq].ForeColor = string_fore_color;        
        editor.Styles[Style.Perl.StringQx].ForeColor = string_fore_color;        
        editor.Styles[Style.Perl.StringQr].ForeColor = string_fore_color;                
        editor.Styles[Style.Perl.StringQw].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.PodVerb].ForeColor = default_word_color;
        editor.Styles[Style.Perl.SubPrototype].ForeColor = default_word_color;
        editor.Styles[Style.Perl.FormatIdent].ForeColor = default_word_color;
        editor.Styles[Style.Perl.Format].ForeColor = default_word_color;
        editor.Styles[Style.Perl.StringVar].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.XLat].ForeColor = default_word_color;
        editor.Styles[Style.Perl.RegexVar].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.RegSubstVar].ForeColor = default_word_color;
        editor.Styles[Style.Perl.BackticksVar].ForeColor = default_word_color;
        editor.Styles[Style.Perl.HereQqVar].ForeColor = default_word_color;
        editor.Styles[Style.Perl.HereQxVar].ForeColor = default_word_color;
        editor.Styles[Style.Perl.StringQqVar].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.StringQxVar].ForeColor = string_fore_color;
        editor.Styles[Style.Perl.StringQrVar].ForeColor = string_fore_color;
        update_keywords(editor,"perl",
            "ADJUST AUTOLOAD BEGIN CHECK DESTROY END INIT UNITCHECK __CLASS__ __DATA__ __END__ __FILE__ __LINE__ __PACKAGE__ __SUB__ abs accept alarm all and any atan2 attributes autodie autouse base bigfloat bigint bignum bigrat bind binmode bless blib break builtin bytes caller catch charnames chdir chmod chomp chop chown chr chroot class close closedir cmp connect constant continue cos crypt dbmclose dbmopen default defer defined delete deprecate diagnostics die do dump each else elseif elsif encoding endgrent endhostent endnetent endprotoent endpwent endservent eof eq eval evalbytes exec exists exit exp experimental fc fcntl feature field fields fileno filetest finally flock for foreach fork format formline ge getc getgrent getgrgid getgrnam gethostbyaddr gethostbyname gethostent getlogin getnetbyaddr getnetbyname getnetent getpeername getpgrp getppid getpriority getprotobyname getprotobynumber getprotoent getpwent getpwnam getpwuid getservbyname getservbyport getservent getsockname getsockopt given glob gmtime goto grep gt hex if import index int integer ioctl isa join keys kill last lc lcfirst le length less lib link listen local locale localtime lock log lstat lt m map meta_notation method mkdir mro msgctl msgget msgrcv msgsnd my ne next no not oct ok open opendir ops or ord our overload overloading pack package parent perlfaq pipe pop pos print printf prototype push q qq qr quotemeta qw qx rand re read readdir readline readlink readpipe recv redo ref rename require reset return reverse rewinddir rindex rmdir s say scalar seek seekdir select semctl semget semop send setgrent sethostent setnetent setpgrp setpriority setprotoent setpwent setservent setsockopt shift shmctl shmget shmread shmwrite shutdown sigtrap sin size sleep socket socketpair sort splice split sprintf sqrt srand stable stat state strict study sub subs substr symlink syscall sysopen sysread sysseek system syswrite tell telldir threads tie tied time times tr truncate try uc ucfirst umask undef unless unlink unpack unshift untie until use utf8 utime values vars vec version vmsish wait waitpid wantarray warn warnings when while write x xor y",
            ""
        );
    }
    public static void set_pascal_style(Scintilla editor) {
        editor.Styles[Style.Pascal.Default].ForeColor = default_word_color;
        editor.Styles[Style.Pascal.Identifier].ForeColor = default_word_color;
        editor.Styles[Style.Pascal.Comment].ForeColor = comment_fore_color;
        editor.Styles[Style.Pascal.Comment].BackColor = comment_back_color;
        editor.Styles[Style.Pascal.Comment2].ForeColor = comment_fore_color;
        editor.Styles[Style.Pascal.Comment2].BackColor = comment_back_color;
        editor.Styles[Style.Pascal.CommentLine].ForeColor = comment_fore_color;
        editor.Styles[Style.Pascal.CommentLine].BackColor = comment_back_color;    
        editor.Styles[Style.Pascal.Preprocessor].ForeColor = preprocessor_color;
        editor.Styles[Style.Pascal.Preprocessor2].ForeColor = preprocessor_color;
        editor.Styles[Style.Pascal.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Pascal.Number].BackColor = number_back_color;
        editor.Styles[Style.Pascal.HexNumber].ForeColor = number_fore_color;
        editor.Styles[Style.Pascal.HexNumber].BackColor = number_back_color;
        editor.Styles[Style.Pascal.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Pascal.String].ForeColor = string_fore_color;
        editor.Styles[Style.Pascal.String].BackColor = string_back_color;
        editor.Styles[Style.Pascal.StringEol].BackColor = Color.Pink;
        editor.Styles[Style.Pascal.Character].ForeColor = string_fore_color;
        editor.Styles[Style.Pascal.Character].BackColor = string_back_color;
        editor.Styles[Style.Pascal.Operator].ForeColor = operator_color;
        editor.Styles[Style.Pascal.Asm].ForeColor = default_word_color;
        update_keywords(editor,"pascal",
            "and array asm begin case cdecl class const constructor default destructor div do downto else end end. except exit exports external far file finalization finally for function goto if implementation in index inherited initialization inline interface label library message mod near nil not object of on or out overload override packed pascal private procedure program property protected public published raise read record register repeat resourcestring safecall set shl shr stdcall stored string then threadvar to try type unit until uses var virtual while with write xor",
            ""
        );
    }
    // >>>
    public static void set_php_style(Scintilla editor) {
        editor.Styles[Style.PhpScript.ComplexVariable].ForeColor = number_fore_color;
        editor.Styles[Style.PhpScript.ComplexVariable].BackColor = number_back_color;
        editor.Styles[Style.PhpScript.Default].ForeColor = default_word_color;
        editor.Styles[Style.PhpScript.HString].ForeColor = string_fore_color;
        editor.Styles[Style.PhpScript.HString].BackColor = string_back_color;
        editor.Styles[Style.PhpScript.SimpleString].ForeColor = string_fore_color;
        editor.Styles[Style.PhpScript.SimpleString].BackColor = string_back_color;
        editor.Styles[Style.PhpScript.Word].ForeColor = keyword1_color;
        editor.Styles[Style.PhpScript.Number].ForeColor = number_fore_color;
        editor.Styles[Style.PhpScript.Number].BackColor = number_back_color;
        editor.Styles[Style.PhpScript.Variable].ForeColor = number_fore_color;
        editor.Styles[Style.PhpScript.Comment].ForeColor = comment_fore_color;
        editor.Styles[Style.PhpScript.Comment].BackColor = comment_back_color;
        editor.Styles[Style.PhpScript.CommentLine].ForeColor = comment_fore_color;
        editor.Styles[Style.PhpScript.CommentLine].BackColor = comment_back_color;
        editor.Styles[Style.PhpScript.HStringVariable].ForeColor = string_fore_color;
        editor.Styles[Style.PhpScript.HStringVariable].BackColor = string_back_color;
        editor.Styles[Style.PhpScript.Operator].ForeColor = operator_color;
        update_keywords(editor,"php",
            "__halt_compiler abstract and array as bool break callable case catch class clone const continue declare default die do echo else elseif empty enddeclare endfor endforeach endif endswitch endwhile enum eval exit extends false final finally float fn for foreach from function global goto if implements include include_once instanceof insteadof int interface isset iterable list match mixed namespace never new null numeric object or print private protected public readonly require require_once resource return static string switch throw trait true try unset use var void while xor yield",
            ""
        );
    }
    public static void set_properties_style(Scintilla editor) {}
    public static void set_ruby_style(Scintilla editor) { // INCOMPLETE
        editor.Styles[Style.Ruby.Default].ForeColor = default_word_color;
        editor.Styles[Style.Ruby.Error].ForeColor = default_word_color;
        editor.Styles[Style.Ruby.CommentLine].ForeColor = comment_fore_color;
        editor.Styles[Style.Ruby.CommentLine].BackColor = comment_back_color;
        editor.Styles[Style.Ruby.Pod].ForeColor = default_word_color;
        editor.Styles[Style.Ruby.Number].ForeColor = number_fore_color;
        editor.Styles[Style.Ruby.Number].BackColor = number_back_color;
        editor.Styles[Style.Ruby.Word].ForeColor = keyword1_color;
        editor.Styles[Style.Ruby.String].ForeColor = string_fore_color;
        editor.Styles[Style.Ruby.String].BackColor = string_back_color;
        editor.Styles[Style.Ruby.Character].ForeColor = string_fore_color;
        editor.Styles[Style.Ruby.Character].BackColor = string_back_color;

        editor.Styles[Style.Ruby.Character].ForeColor = string_fore_color;
        editor.Styles[Style.Ruby.Character].ForeColor = string_fore_color;

//        editor.Styles[Style.Ruby.Punctuation].ForeColor = default_word_color;
//        editor.Styles[Style.Ruby.Preprocessor].ForeColor = preprocessor_color;
        editor.Styles[Style.Ruby.Operator].ForeColor = operator_color;
        editor.Styles[Style.Ruby.Identifier].ForeColor = default_word_color;

        editor.Styles[Style.Ruby.Regex].ForeColor = string_fore_color;
        editor.Styles[Style.Ruby.Regex].BackColor = string_back_color;

    }
    public static void set_smalltalk_style(Scintilla editor) {}
    public static void set_sql_style(Scintilla editor) {}
    public static void set_r_style(Scintilla editor) {}
    public static void set_vb_style(Scintilla editor) {}
    public static void set_vbscript_style(Scintilla editor) {}
    public static void set_verilog_style(Scintilla editor) {}
    public static void set_markdown_style(Scintilla editor) {}
    // <<< 

    public static void post_styling_comment_line(Scintilla editor, string marker) { // TESTING
        const int INDICATOR_ID = 15; // pick a free one

        // Configure indicator once (you should ideally move this elsewhere)
        editor.Indicators[INDICATOR_ID].Style = IndicatorStyle.TextFore;
        editor.Indicators[INDICATOR_ID].ForeColor = comment_fore_color; // comment-like
        // editor.Indicators[INDICATOR_ID].BackColor = comment_back_color; // comment-like
        editor.Indicators[INDICATOR_ID].Under = false;

        editor.IndicatorCurrent = INDICATOR_ID;
        editor.IndicatorClearRange(0, editor.TextLength);
        foreach (var line in editor.Lines) {
            string text = line.Text;
            int idx = text.IndexOf(marker);
            if (idx >= 0) {
                int start = line.Position + idx;
                int length = text.Length - idx;
                editor.IndicatorFillRange(start, length);
            }
        }
    }

    // variables -- custom highlight 
    private static string textmarker_pattern = @"\{Notepad--T;([^}]*)\}";
    private static Regex? textmarker_pattern_regex = null;
    private static string highlight_override_pattern = @"\{Notepad--H;([^}]*)\}";
    private static Regex? highlight_override_pattern_regex = null;
    private static string lexer_override_pattern = @"\{Notepad--L:([^}]*)\}";
    private static Regex? lexer_override_pattern_regex = null;
    private static string search_token_pattern = @"\{Notepad--S:([^}]*)\}"; 
    private static Regex? search_token_pattern_regex = null; 
    private static int line_count_for_directive_search = 100;
    // variables | methods -- custom highlight 
    public static void apply_textmarker_highlight_for_file_directives(Scintilla editor) {
        if (textmarker_pattern_regex==null) textmarker_pattern_regex = new Regex(textmarker_pattern);
        // Clear all indicators you plan to use
        for (int i = 8; i < 15; i++) { 
            editor.IndicatorCurrent = i;
            editor.IndicatorClearRange(0, editor.TextLength); 
        }
        int indicatorIndex = 8; // start at 8, go up for each color
        for (int i = 0; i < Math.Min(line_count_for_directive_search, editor.Lines.Count); i++) {
            var lineText = editor.Lines[i].Text;
            var match = textmarker_pattern_regex.Match(lineText);
            if (!match.Success) continue;
            var directives = match.Groups[1].Value.Split(';');
            foreach (var directive in directives) {
                var parts = directive.Split(':');
                if (parts.Length != 2) continue;
                var colorName = parts[0].Trim();
                var keywords = parts[1]
                    .Split(',')
                    .Select(k => k.Trim())
                    .Where(k => k.Length > 0);
                Color c = Color.FromName(colorName);
                // Configure this indicator
                editor.Indicators[indicatorIndex].Style = IndicatorStyle.StraightBox;
                editor.Indicators[indicatorIndex].ForeColor = c;
                editor.Indicators[indicatorIndex].Alpha = 60;
                editor.Indicators[indicatorIndex].OutlineAlpha = 255;
                foreach (var keyword in keywords){
                    int pos = 0;
                    while ((pos = editor.Text.IndexOf(keyword, pos, StringComparison.Ordinal)) != -1) {
                        editor.IndicatorCurrent = indicatorIndex;
                        editor.IndicatorFillRange(pos, keyword.Length);
                        pos += keyword.Length;
                    }
                }
                indicatorIndex++; // next color → next indicator
            }
        }
    }
    public static void apply_custom_highlight_override_for_file_directives(Scintilla editor) {
        if (highlight_override_pattern_regex==null) highlight_override_pattern_regex = new Regex(highlight_override_pattern);
        bool has_match = false;
        for (int i = 0; i < Math.Min(line_count_for_directive_search, editor.Lines.Count); i++) {
            var lineText = editor.Lines[i].Text;
            var match = highlight_override_pattern_regex.Match(lineText);
            if (!match.Success) continue;
            has_match = true;
            var directives = match.Groups[1].Value.Split(';');
            foreach (var directive in directives) {
                var parts = directive.Split(':');
                if (parts.Length != 2) continue;
                var lexerComponentName = parts[0].Trim();
                var colorName = parts[1].Trim();
                apply_lexer_color_directive(lexerComponentName, colorName);
            }
        }
        if (!has_match) reset_global_dark_theme_colors();
    }
    private static void apply_lexer_color_directive(string lexer_component, string color_name) { // REVISION
        switch (lexer_component) {
            case "1":
            case "keyword1":
                keyword1_color = Color.FromName(color_name);
                break;
            case "2":
            case "keyword2":
                keyword2_color = Color.FromName(color_name);
                break;
        }
    }
    public static string apply_lexer_override_directive(Scintilla editor, string filename) {
        if (lexer_override_pattern_regex==null) lexer_override_pattern_regex = new Regex(lexer_override_pattern);
        for (int i = 0; i < Math.Min(line_count_for_directive_search, editor.Lines.Count); i++) {
            var lineText = editor.Lines[i].Text;
            var match = lexer_override_pattern_regex.Match(lineText);
            if (!match.Success) continue;
            string directive = match.Groups[1].Value;
            if ( string.IsNullOrWhiteSpace(directive) ) continue;
            return directive;
        }
        return filename;
    }

    // -- comment helpers 
    public static void toggle_comment_lines(Scintilla editor) {
        if (editor == null) return;
        string commentString = GetLineCommentString(editor.LexerName);
        if (string.IsNullOrEmpty(commentString)) return;

        int startLine = editor.LineFromPosition(editor.SelectionStart);
        int endLine = editor.LineFromPosition(editor.SelectionEnd);

        editor.BeginUndoAction();
//        editor.SuspendDrawing(); // optional, reduces flicker

        try
        {
            for (int line = startLine; line <= endLine; line++)
            {
                var sciLine = editor.Lines[line];
                string text = editor.GetTextRange(sciLine.Position, sciLine.Length);

                if (text.TrimStart().StartsWith(commentString))
                {
                    // Remove comment prefix
                    int pos = sciLine.Position + text.IndexOf(commentString);
                    editor.TargetStart = pos;
                    editor.TargetEnd = pos + commentString.Length;
                    editor.ReplaceTarget(string.Empty);
                }
                else
                {
                    // Add comment prefix
                    editor.TargetStart = sciLine.Position;
                    editor.TargetEnd = sciLine.Position;
                    editor.ReplaceTarget(commentString);
                }
            }
        }
        finally
        {
//            editor.ResumeDrawing();
            editor.EndUndoAction();
        }
    }
    public static void comment_out(Scintilla editor) {
        if (editor == null) return;
        string commentString = GetLineCommentString(editor.LexerName);
        if (string.IsNullOrEmpty(commentString)) return; // No line comment style available
        int startLine = editor.LineFromPosition(editor.SelectionStart);
        int endLine = editor.LineFromPosition(editor.SelectionEnd);
        editor.BeginUndoAction();
        try {
            for (int line = startLine; line <= endLine; line++) {
                int pos = editor.Lines[line].Position;
                editor.InsertText(pos, commentString);
            }
        }
        finally {
            editor.EndUndoAction();
        }
    }
    private static string GetLineCommentString(string lexerName) {
        switch (lexerName.ToLowerInvariant())
        {
            case "cpp":
            case "cs":
            case "java":
                return "//";
            case "python":
            case "perl":
            case "ruby":
                return "#";
            case "sql":
                return "--";
            case "lua":
                return "--";
            default:
                return null;
        }
    }
	
	// -- fold helpers
	public static void fold_all(Scintilla editor) { // REVISION
		const int SCI_FOLDALL = 2662;
		const int SC_FOLDACTION_CONTRACT = 0;
		editor.DirectMessage(SCI_FOLDALL, (IntPtr)SC_FOLDACTION_CONTRACT);
        // -- 
        int lineCount = (int)editor.DirectMessage(2154); // SCI_GETLINECOUNT
		for (int i = 0; i < lineCount; i++) {
			int level = (int)editor.DirectMessage(2223, (IntPtr)i); // SCI_GETFOLDLEVEL
			if ((level & 0x2000) != 0) // SC_FOLDLEVELHEADERFLAG
			{
				editor.DirectMessage(2237, (IntPtr)i, (IntPtr)0); // SCI_SETFOLDEXPANDED false
				int lastChild = (int) editor.DirectMessage(2224, (IntPtr)i, (IntPtr)(-1)); // SCI_GETLASTCHILD
				editor.DirectMessage(2227, (IntPtr)(i + 1), (IntPtr)lastChild); // SCI_HIDELINES
			}
		}
	}
    public static void unfold_all(Scintilla editor) {
        const int SCI_FOLDALL = 2662;
        const int SC_FOLDACTION_EXPAND = 1;
        editor.DirectMessage(SCI_FOLDALL, (IntPtr)SC_FOLDACTION_EXPAND);
        // --
        int lineCount = (int)editor.DirectMessage(2154); // SCI_GETLINECOUNT
        for (int i = 0; i < lineCount; i++) {
            int level = (int)editor.DirectMessage(2223, (IntPtr)i); // SCI_GETFOLDLEVEL
            if ((level & 0x2000) != 0) // SC_FOLDLEVELHEADERFLAG
            {
                editor.DirectMessage(2237, (IntPtr)i, (IntPtr)1); // SCI_SETFOLDEXPANDED true
                int lastChild = (int)editor.DirectMessage(2224, (IntPtr)i, (IntPtr)(-1)); // SCI_GETLASTCHILD
                editor.DirectMessage(2226, (IntPtr)(i + 1), (IntPtr)lastChild); // SCI_SHOWLINES
            }
        }
    }
    public static void smart_fold_all(Scintilla editor) { // REVISION
        if (editor == null) return;
        const int SCI_GETLINECOUNT = 2154;
        const int SCI_LINEFROMPOSITION = 2166;
        const int SCI_GETFOLDLEVEL = 2223;
        const int SCI_SETFOLDEXPANDED = 2237;
        const int SCI_GETLASTCHILD = 2224;
        const int SCI_HIDELINES = 2227;
        const int SC_FOLDLEVELHEADERFLAG = 0x2000;
        const int SC_FOLDLEVELNUMBERMASK = 0x0FFF;
        int lineCount = (int)editor.DirectMessage(SCI_GETLINECOUNT);
        // --- detect caret fold level ---
        int caretPos = editor.CurrentPosition;
        int caretLine = (int)editor.DirectMessage(SCI_LINEFROMPOSITION, (IntPtr)caretPos);
        int caretLevel = (int)editor.DirectMessage(SCI_GETFOLDLEVEL, (IntPtr)caretLine);
        caretLevel &= SC_FOLDLEVELNUMBERMASK;
        // --- fold everything deeper than caret level ---
        for (int i = 0; i < lineCount; i++) {
            int level = (int)editor.DirectMessage(SCI_GETFOLDLEVEL, (IntPtr)i);

            if ((level & SC_FOLDLEVELHEADERFLAG) != 0) {
                int indent = level & SC_FOLDLEVELNUMBERMASK;

                if (indent >= caretLevel) {
                    editor.DirectMessage(SCI_SETFOLDEXPANDED, (IntPtr)i, IntPtr.Zero);

                    int lastChild = (int)editor.DirectMessage(
                        SCI_GETLASTCHILD,
                        (IntPtr)i,
                        (IntPtr)(-1)
                    );

                    editor.DirectMessage(
                        SCI_HIDELINES,
                        (IntPtr)(i + 1),
                        (IntPtr)lastChild
                    );
                }
            }
        }
    }
    public static void toggle_fold_marker(Scintilla editor) {
        int line = get_current_caret_line(editor);
        if (line >= 0) editor.DirectMessage(NM.SCI_TOGGLEFOLD, (IntPtr)line); 
    }
    public static int get_current_caret_line(Scintilla editor) {
        if (editor == null) return -1;
        int pos = (int)editor.DirectMessage(NM.SCI_GETCURRENTPOS); 
        int line = (int)editor.DirectMessage(NM.SCI_LINEFROMPOSITION, (IntPtr)pos); 
        return line;
    }    
    public static void unfold_line(Scintilla editor, int line) {
        if (editor == null) return;
        editor.DirectMessage(NM.SCI_SETFOLDEXPANDED, (IntPtr)line, (IntPtr)1);
        // show the lines hidden under this fold
        int lastChild = (int)editor.DirectMessage(NM.SCI_GETLASTCHILD, (IntPtr)line); 
        if (lastChild > line) {
            editor.DirectMessage(NM.SCI_SHOWLINES, (IntPtr)(line + 1), (IntPtr)lastChild); 
        }
    }
    public static void unfold_line(Scintilla editor) {
        int line = get_current_caret_line(editor);
        if (line<0) return ;
        unfold_line(editor, line);
    }
    public static void fold_line(Scintilla editor, int line) {}
    public static void fold_line(Scintilla editor) {}

    // -- find helpers 
    /* Logic [ Search Tokens ]
    Incantation_SCINTILLA.set_keyshortcuts() || sets Ctrl+F and Ctrl+D logic 
    Ctrl+F || $token | %* input_dialog | %* | find_next_token()
    Ctrl+D || $token | %* input_dialog | find_prev_token()
    ... || $token | %* input_dialog || input_dialog() 
    ... || $token || get_selected_token() || get selected text | !% get search token by directive | %* | ... 
    ... || $token || ... | !% get search token by directive || get_selected_token_by_directive() 
    GSTD := get_selected_token_by_directive 
    GSTD() || &f document head lines || %f directive match || parse and return token 
    1. Selected text has priority 
    2. Second priority is the directive token 
    3. Third attempt is dialog 
    */
    public static string get_selected_token_by_directive(Scintilla editor) { 
        if ( search_token_pattern_regex==null ) search_token_pattern_regex = new Regex( search_token_pattern );
        string token = "";
        for (int i = 0; i < Math.Min(line_count_for_directive_search, editor.Lines.Count); i++) {
            var lineText = editor.Lines[i].Text;
            var match = search_token_pattern_regex.Match(lineText);
            if (!match.Success) continue;
            token = match.Groups[1].Value;
            if ( string.IsNullOrWhiteSpace(token) ) continue;
            break;
        }
        return token;
    }
    public static string get_selected_token(Scintilla editor) {
        if (editor == null) return "";
        string text = editor.SelectedText;
        if (string.IsNullOrWhiteSpace(text)) text = get_selected_token_by_directive(editor);
        if (string.IsNullOrWhiteSpace(text)) return "";
        text = text.TrimStart();
        int i = 0;
        while (i < text.Length && !char.IsWhiteSpace(text[i])) {
            i++;
        }
        return text.Substring(0, i);
    }
    public static void find_next_token(Scintilla editor, string token) {
        if (editor == null || string.IsNullOrEmpty(token)) return;
        unfold_all(editor);
        int start = editor.CurrentPosition;
        int end = editor.TextLength;
        editor.TargetStart = start;
        editor.TargetEnd = end;
        int pos = editor.SearchInTarget(token);
        if (pos != -1) {
            editor.SetSelection(pos + token.Length, pos);               
            editor.ScrollCaret();
            smart_fold_all(editor);

            int line = editor.LineFromPosition(editor.CurrentPosition);
            editor.DirectMessage(2234, (IntPtr)line, IntPtr.Zero); // Ensure visible
        }
    }
    public static void find_prev_token(Scintilla editor, string token) {
        if (editor == null || string.IsNullOrEmpty(token)) return;
        unfold_all(editor);
        const int SCFIND_WHOLEWORD = 2;
        int caret = editor.CurrentPosition;
        // Find bounds of the word under the caret
        int wordStart = (int)editor.DirectMessage(2266, (IntPtr)caret, (IntPtr)1); // SCI_WORDSTARTPOSITION
        int wordEnd   = (int)editor.DirectMessage(2267, (IntPtr)caret, (IntPtr)1); // SCI_WORDENDPOSITION
        int searchEnd = wordStart; // exclude current word completely
        editor.SearchFlags = (ScintillaNET.SearchFlags) SCFIND_WHOLEWORD;
        editor.TargetStart = 0;
        editor.TargetEnd = searchEnd;
        int lastPos = -1;
        while (true) {
            int pos = editor.SearchInTarget(token);
            if (pos == -1) break;
            lastPos = pos;
            editor.TargetStart = pos + token.Length;
            editor.TargetEnd = searchEnd;
        }
        if (lastPos != -1) {
            editor.SetSelection(lastPos + token.Length, lastPos);
            editor.ScrollCaret();
            smart_fold_all(editor);

            int line = editor.LineFromPosition(editor.CurrentPosition);
            editor.DirectMessage(2234, (IntPtr)line, IntPtr.Zero); // Ensure visible
        }
    }
    
    // REVISION
    // Define custom marker indices for begin/end folds
    private const int BeginFoldMarker = 20; // pick an unused marker index
    private const int EndFoldMarker   = 21;
    public static void add_begin_fold_marker(Scintilla editor) {
        int line = editor.CurrentLine;
        editor.Lines[line].MarkerAdd(BeginFoldMarker);
    }
    public static void add_end_fold_marker(Scintilla editor) {
        int line = editor.CurrentLine;
        editor.Lines[line].MarkerAdd(EndFoldMarker);
    }
    public static void remove_fold_marker(Scintilla editor) {
        int line = editor.CurrentLine;
        editor.Lines[line].MarkerDelete(BeginFoldMarker);
        editor.Lines[line].MarkerDelete(EndFoldMarker);
    }
    public static void configure_manual_fold_markers(Scintilla editor) {
        editor.Markers[BeginFoldMarker].Symbol = MarkerSymbol.Arrow;
        editor.Markers[BeginFoldMarker].SetForeColor(System.Drawing.Color.White);
        editor.Markers[BeginFoldMarker].SetBackColor(System.Drawing.Color.Green);
        editor.Markers[EndFoldMarker].Symbol = MarkerSymbol.ArrowDown;
        editor.Markers[EndFoldMarker].SetForeColor(System.Drawing.Color.White);
        editor.Markers[EndFoldMarker].SetBackColor(System.Drawing.Color.Red);
    }

    // debug 
    public static void dump_lexer_names(Scintilla editor){
        editor.AppendText("=== Lexer Names ===\n");
        int count = 1;
        foreach( string name in Lexilla.GetLexerNames() ) {
            editor.AppendText(to_string(count)+". "+name+"\n");
            count++;
        }
    }

    // DEPRECATED - but you should confirm if no other function use this!
    public static void set_language_folding(Scintilla scintilla, string lexer) {
		// --
		// Set the lexer
		scintilla.LexerName = lexer;
		// Instruct the lexer to calculate folding
		scintilla.SetProperty("fold", "1");
		scintilla.SetProperty("fold.compact", "0"); // TESTING 
        scintilla.SetProperty("fold.comment", "1");
		// Configure a margin to display folding symbols
		scintilla.Margins[2].Type = MarginType.Symbol;
		scintilla.Margins[2].Mask = Marker.MaskFolders;
		scintilla.Margins[2].Sensitive = true;
		scintilla.Margins[2].Width = 20;
		// Set colors for all folding markers
		for (int i = 25; i <= 31; i++) {
			scintilla.Markers[i].SetForeColor(fold_fore_color);
			scintilla.Markers[i].SetBackColor(fold_back_color);
		}
		
		// Marker colors
		Color fore = fold_fore_color;
		Color back = fold_back_color;
		scintilla.Markers[Marker.Folder].SetForeColor(fore);
		scintilla.Markers[Marker.Folder].SetBackColor(back);
		scintilla.Markers[Marker.FolderOpen].SetForeColor(fore);
		scintilla.Markers[Marker.FolderOpen].SetBackColor(back);
		scintilla.Markers[Marker.FolderEnd].SetForeColor(fore);
		scintilla.Markers[Marker.FolderEnd].SetBackColor(back);
		scintilla.Markers[Marker.FolderMidTail].SetForeColor(fore);
		scintilla.Markers[Marker.FolderMidTail].SetBackColor(back);
		scintilla.Markers[Marker.FolderOpenMid].SetForeColor(fore);
		scintilla.Markers[Marker.FolderOpenMid].SetBackColor(back);
		scintilla.Markers[Marker.FolderSub].SetForeColor(fore);
		scintilla.Markers[Marker.FolderSub].SetBackColor(back);
		scintilla.Markers[Marker.FolderTail].SetForeColor(fore);
		scintilla.Markers[Marker.FolderTail].SetBackColor(back);
		
		// Folding margin background
		scintilla.SetFoldMarginColor(true, Color.FromArgb(30, 30, 30));
		scintilla.SetFoldMarginHighlightColor(true, Color.FromArgb(30, 30, 30));
		
		// Configure folding markers with respective symbols
		scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
		scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
		scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
		scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
		scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
		scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
		scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;
		// Enable automatic folding
		scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
	}



}

// -- END 