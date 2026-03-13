namespace Notepad__;
using Codex;
using static Codex.Incantation;

static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Notepad__Form NPF = new Notepad__Form();
        register_icon(NPF, "Notepad__", "Notepad__");
        Application.Run(NPF);
    }    
}