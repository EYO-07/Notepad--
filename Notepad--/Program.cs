namespace Notepad__;
using Codex;
using static Codex.Incantation;
//using static Codex.Conjuration_GLOBALHOTKEY;

static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
//        Application.ApplicationExit += (s, e) => { unregister_all_hotkeys(); };
//        AppDomain.CurrentDomain.UnhandledException += (s, e) => { unregister_all_hotkeys(); };
//        Application.ThreadException += (s, e) => { unregister_all_hotkeys(); };
        Notepad__Form NPF = new Notepad__Form();
        register_icon(NPF, "Notepad__", "Notepad__");
        Application.Run(NPF);
    }    
}