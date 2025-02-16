using System.Windows.Forms;
using System.Drawing;
using Tetris.UI;

namespace Tetris;

public class GameLogic : GameLogicBase
{
    public GameLogic(IGameView view)
        : base(view)
    {
        View.Level = 1;
        View.Lines = 2;
        View.Score = 3;
        TimerInterval = 500;

        for (var y = 0; y < 20; ++y)
        {
            for (var x = 0; x < 10; ++x)
            {
                View.SetPixelMainArea(x, y, Color.Red);
            }
        }

        for (var y = 0; y < 4; ++y)
        {
            for (var x = 0; x < 4; ++x)
            {
                View.SetPixelHelpArea(x, y, Color.Red);
            }
        }
    }

    protected override void OnTimerTick()
    {
    }

    protected override void OnKeyPressed(Keys key)
    {
    }
}