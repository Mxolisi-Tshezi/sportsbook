using Timer = System.Windows.Forms.Timer;

namespace Tetris.UI;

public sealed class GameView : Form, IGameView
{
    private const int MainAreaWidth = 10;
    private const int MainAreaHeight = 20;
    private const int HelpAreaWidth = 4;
    private const int HelpAreaHeight = 4;

    private readonly Timer redrawTimer;
    private readonly Color[,] mainArea;
    private readonly Color[,] helpArea;

    public int Score { get; set; }
    public int Level { get; set; }
    public int Lines { get; set; }
    private GameViewConfig Config { get; }

    public GameView(GameViewConfig config)
    {
        Config = config;
        ClientSize = new Size(Config.ScreenWidth, Config.ScreenHeight);
        DoubleBuffered = true;
        redrawTimer = new Timer();

        mainArea = new Color[MainAreaWidth, MainAreaHeight];
        helpArea = new Color[HelpAreaWidth, HelpAreaHeight];
    }

    public void Initialize()
    {
        redrawTimer.Interval = 1000 / Config.Fps;
        redrawTimer.Tick += OnRedraw;
        redrawTimer.Start();
    }

    public void Shutdown()
    {
        redrawTimer.Stop();
        redrawTimer.Tick -= OnRedraw;
    }

    public void SetPixelMainArea(int x, int y, Color color)
    {
        mainArea[x, y] = color;
    }

    public void SetPixelHelpArea(int x, int y, Color color)
    {
        helpArea[x, y] = color;
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        // Clear the window.
        using (var backgroundBrush = new SolidBrush(Color.Black))
        {
            pe.Graphics.FillRectangle(backgroundBrush, ClientRectangle);
        }

        // Draw heads-up display (score, level, line count etc).
        DrawHud(pe.Graphics, Score, Level, Lines);

        var mainAreaRectangle = GetMainAreaRectangle();
        var helpAreaRectangle = GetHelpAreaRectangle();

        // Fill the main window cells with random colors.
        for (var y = 0; y < MainAreaHeight; ++y)
        {
            for (var x = 0; x < MainAreaWidth; ++x)
            {
                DrawCell(pe.Graphics, mainAreaRectangle, x, y, mainArea[x, y]);
            }
        }

        // Fill the next piece window cells with random colors.
        for (var y = 0; y < HelpAreaHeight; ++y)
        {
            for (var x = 0; x < HelpAreaWidth; ++x)
            {
                DrawCell(pe.Graphics, helpAreaRectangle, x, y, helpArea[x, y]);
            }
        }
    }

    // Draws a cell in the specified rectangle, using the specified color.
    private void DrawCell(Graphics graphics, Rectangle rectangle, int x, int y, Color color)
    {
        var block = new Rectangle
        {
            Width = Config.BlockSize,
            Height = Config.BlockSize,
            X = Config.BlockPadding + rectangle.X + (x * (Config.BlockSize + Config.BlockPadding)),
            Y = Config.BlockPadding + rectangle.Y + (y * (Config.BlockSize + Config.BlockPadding))
        };

        using (var brush = new SolidBrush(color))
        {
            graphics.FillRectangle(brush, block);
        }
    }

    // Draws the heads-up display.
    private void DrawHud(Graphics graphics, int score, int level, int lineCount)
    {
        var mainAreaRectangle = GetMainAreaRectangle();
        var helpAreaRectangle = GetHelpAreaRectangle();

        mainAreaRectangle = new Rectangle(mainAreaRectangle.X - 1, mainAreaRectangle.Y - 1, mainAreaRectangle.Width + 1, mainAreaRectangle.Height + 1);
        helpAreaRectangle = new Rectangle(helpAreaRectangle.X - 1, helpAreaRectangle.Y - 1, helpAreaRectangle.Width + 1, helpAreaRectangle.Height + 1);

        // Draw border.
        graphics.DrawRectangle(Pens.White, mainAreaRectangle);

        using (var drawFont = new Font("Georgia", 12))
        using (var drawBrush = new SolidBrush(Color.White))
        using (var drawFormat = new StringFormat())
        {
            // Draw score.
            var drawString = string.Format("Score: {0}", score);
            graphics.DrawString(drawString, drawFont, drawBrush, ClientRectangle, drawFormat);

            // Draw level.
            drawString = string.Format("Level: {0}", level);
            graphics.DrawString(drawString, drawFont, drawBrush, 0.0F, ClientRectangle.Bottom - 30.0F, drawFormat);

            // Draw line count.
            drawFormat.Alignment = StringAlignment.Far;
            drawString = string.Format("Lines: {0}", lineCount);
            graphics.DrawString(drawString, drawFont, drawBrush, ClientRectangle, drawFormat);

            // Draw next piece text.
            drawString = string.Format("Next:");
            graphics.DrawString(drawString, drawFont, drawBrush, helpAreaRectangle.X, helpAreaRectangle.Y - 20);

            // Draw next piece rectangle.
            graphics.DrawRectangle(Pens.White, helpAreaRectangle);
        }
    }

    private Rectangle GetMainAreaRectangle()
    {
        var width = Config.BlockPadding + ((Config.BlockSize + Config.BlockPadding) * MainAreaWidth);
        var height = Config.BlockPadding + ((Config.BlockSize + Config.BlockPadding) * MainAreaHeight);
        var x = (ClientRectangle.Width - width) / 2;
        var y = (ClientRectangle.Height - height) / 2;
        return new Rectangle(new Point(x, y), new Size(width, height));
    }

    private Rectangle GetHelpAreaRectangle()
    {
        var width = Config.BlockPadding + ((Config.BlockSize + Config.BlockPadding) * HelpAreaWidth);
        var height = Config.BlockPadding + ((Config.BlockSize + Config.BlockPadding) * HelpAreaHeight);
        var x = ClientRectangle.Right - 150;
        var y = ClientRectangle.Bottom / 2 - 40;
        return new Rectangle(new Point(x, y), new Size(width, height));
    }

    private void OnRedraw(object sender, EventArgs e)
    {
        Invalidate(); // Causes OnPaint() to be called.
    }
}