namespace Tetris.UI
{
    public interface IGameView
    {
        int Score { get; set; }
        int Level { get; set; }
        int Lines { get; set; }

        event KeyEventHandler KeyDown;

        void SetPixelMainArea(int x, int y, Color color);
        void SetPixelHelpArea(int x, int y, Color color);
    }
}
