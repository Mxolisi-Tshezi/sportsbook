using System;
using System.Windows.Forms;
using Tetris.UI;

namespace Tetris;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var view = new GameView(new GameViewConfig());
        var logic = new GameLogic(view);
        view.Initialize();
        Application.Run(view);
    }
}