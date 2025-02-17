using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tetris.UI;

namespace Tetris;

public class GameLogic : GameLogicBase
{
    private const int Rows = 20;
    private const int Columns = 10;
    private Color[,] grid;
    private Tetrimino currentPiece;
    private Random random;
    private Tetrimino nextPiece;

    private int tickCounter = 0;
    private const int moveThreshold = 3; //Slowing down the time

    public GameLogic(IGameView view) : base(view)
    {
        grid = new Color[Columns, Rows];
        random = new Random();
        Console.WriteLine("Game Initialized");

        SpawnNewPiece();

    }

    private void SpawnNewPiece()
    {
        if (nextPiece == null) // First-time game start
        {
            nextPiece = new Tetrimino(random.Next(7));
        }

        currentPiece = nextPiece; // Use the stored piece
        nextPiece = new Tetrimino(random.Next(7)); // Generate new next piece

        Console.WriteLine($"New Piece Spawned at {currentPiece.Position}");

        if (!IsColliding(currentPiece, 0, 0))
        {
            DrawPiece(currentPiece, true);
        }

        // Display next piece by manually drawing it in the "Next" section
        DisplayNextPiece();


    }


    private void DisplayNextPiece()
    {
        if (nextPiece == null) return;

        // Clear the preview area before drawing the new piece
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                View.SetPixelHelpArea(x, y, Color.Black); // Clear the area
            }
        }

        // Draw the next piece in the preview area
        foreach (var block in nextPiece.GetBlocks())
        {
            int x = block.X;
            int y = block.Y;
            View.SetPixelHelpArea(x, y, nextPiece.Color); // Draw new piece
        }
    }



    protected override void OnTimerTick()
    {
        Console.WriteLine("Timer Tick");
        tickCounter++; // Increment the counter
        Console.WriteLine($"Timer Tick: {tickCounter}");

        if (tickCounter >= moveThreshold)
        {
            MovePiece(0, 1);
            tickCounter = 0; // Reset counter after moving
        }
    }

    protected override void OnKeyPressed(Keys key)
    {
        Console.WriteLine($"Key Pressed: {key}");

        switch (key)
        {
            case Keys.Left:
                MovePiece(-1, 0);
                break;
            case Keys.Right:
                MovePiece(1, 0);
                break;
            case Keys.Down:
                MovePiece(0, 1);
                break;
            case Keys.Up:
                RotatePiece();
                break;
        }
    }

    private bool IsColliding(Tetrimino piece, int dx, int dy)
    {
        foreach (var block in piece.GetBlocks())
        {
            int x = block.X + piece.Position.X + dx;
            int y = block.Y + piece.Position.Y + dy;

            if (x < 0 || x >= Columns || y < 0 || y >= Rows || grid[x, y] != Color.Empty)

                return true;
            Console.WriteLine("Collision detected");

        }
        return false;
    }

    private void MovePiece(int dx, int dy)
    {
        if (!IsColliding(currentPiece, dx, dy))
        {
            DrawPiece(currentPiece, false);
            currentPiece.Move(dx, dy);
            DrawPiece(currentPiece, true);
        }
        else if (dy > 0)
        {
            Console.WriteLine("Piece Locked");

            LockPiece();
        }
    }

    private void RotatePiece()
    {
        Console.WriteLine("Rotating Piece");

        currentPiece.Rotate();
        if (IsColliding(currentPiece, 0, 0))
        {
            Console.WriteLine("Rotation Collision, Reverting");

            currentPiece.RotateBack();
        }
    }

    private void LockPiece()
    {
        foreach (var block in currentPiece.GetBlocks())
        {
            int x = block.X + currentPiece.Position.X;
            int y = block.Y + currentPiece.Position.Y;
            grid[x, y] = currentPiece.Color;
        }
        ClearFullLines();
        SpawnNewPiece();
    }

    private void ClearFullLines()
    {
        for (int y = 0; y < Rows; y++)
        {
            bool fullLine = true;
            for (int x = 0; x < Columns; x++)
            {
                if (grid[x, y] == Color.Empty)
                {
                    fullLine = false;
                    break;
                }
            }
            if (fullLine)
            {
                ShiftLinesDown(y);
                View.Score += 100;
            }
        }
    }

    private void ShiftLinesDown(int clearedRow)
    {
        for (int y = clearedRow; y > 0; y--)
        {
            for (int x = 0; x < Columns; x++)
            {
                grid[x, y] = grid[x, y - 1];
            }
        }
    }

    private void DrawPiece(Tetrimino piece, bool place)
    {
        foreach (var block in piece.GetBlocks())
        {
            int x = block.X + piece.Position.X;
            int y = block.Y + piece.Position.Y;
            View.SetPixelMainArea(x, y, place ? piece.Color : Color.Empty);
        }
    }
}

public class Tetrimino
{
    public Point Position { get; private set; }
    public Color Color { get; private set; }
    private Point[] blocks;
    private int[,] shape;

    public Tetrimino(int type)
    {
        Position = new Point(3, 0);
        Color = GetColor(type);
        shape = GetShape(type);
    }

    public IEnumerable<Point> GetBlocks()
    {
        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (shape[y, x] == 1)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }

    public void Move(int dx, int dy)
    {
        Position = new Point(Position.X + dx, Position.Y + dy);
    }

    public void Rotate()
{
    int rows = shape.GetLength(0);
    int cols = shape.GetLength(1);
    int[,] rotated = new int[cols, rows]; // Swap rows and columns

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            rotated[x, rows - 1 - y] = shape[y, x]; // Adjust indexing dynamically
        }
    }

    shape = rotated;
}


    public void RotateBack()
    {
        Rotate(); Rotate(); Rotate();
    }

    private Color GetColor(int type)
    {
        Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Cyan, Color.Orange };
        return colors[type % colors.Length];
    }

    private int[,] GetShape(int type)
    {
        int[][,] shapes = {
        new int[,] { { 1, 1, 1, 1 } }, // I piece
        new int[,] { { 1, 1 }, { 1, 1 } }, // O piece
        new int[,] { { 0, 1, 0 }, { 1, 1, 1 } }, // T piece
        new int[,] { { 1, 0 }, { 1, 1 }, { 0, 1 } }, // S piece
        new int[,] { { 0, 1 }, { 1, 1 }, { 1, 0 } }, // Z piece
        new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }, // J piece
        new int[,] { { 0, 1, 1 }, { 1, 1, 0 } } // L piece
    };

        return shapes[type % shapes.Length];
    }
}
