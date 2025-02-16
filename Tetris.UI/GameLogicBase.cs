using System.Timers;
using Timer = System.Timers.Timer;

namespace Tetris.UI;

public abstract class GameLogicBase
{
    private readonly Timer timer;

    protected double TimerInterval
    {
        set => timer.Interval = value;
    }

    protected IGameView View { get; }

    protected GameLogicBase(IGameView view)
    {
        View = view;
        View.KeyDown += OnKeyDown;

        timer = new Timer();
        timer.Elapsed += OnTimerElapsed;
        timer.Start();
    }

    protected abstract void OnTimerTick();
    protected abstract void OnKeyPressed(Keys key);

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        OnKeyPressed(e.KeyCode);
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        OnTimerTick();
    }
}