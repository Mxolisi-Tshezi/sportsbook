namespace Tetris.UI
{
    public class GameViewConfig
    {
        public int Fps { get; private set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public int BlockSize { get; private set; }
        public int BlockPadding { get; private set; }
        
        public GameViewConfig()
        {
            Fps = 60;

            ScreenWidth = 640;
            ScreenHeight = 480;

            BlockSize = 14; // Block size in pixels.
            BlockPadding = 1; // Padding in pixels.
        }
    }
}
