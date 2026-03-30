namespace Notepad__;
using Codex;
using static Codex.Incantation;
using System;
using System.Threading;
using System.Windows.Forms;

static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        bool createdNew;
        using (Mutex mutex = new Mutex(true, "Notepad--Scintilla5WinformCodeEditor", out createdNew)) {
            if (!createdNew) {
                MessageBox.Show("Application is already running.");
                return;
            }
            ApplicationConfiguration.Initialize();
            Notepad__Form NPF = new Notepad__Form();
            register_icon(NPF, "Notepad__", "Notepad__");
            Application.Run(NPF);
        }
    }    
}