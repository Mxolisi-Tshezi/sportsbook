using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tetris.UI;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tetris;


//FEATURES IMPLEMENTED
// Tetris Piece Management
// Collision Detection & Handling
// Removing Full Lines & Shifting Blocks
// Game Scoring System
// Game Over Condition
// Next Piece Preview
// Game Restart Functionality by Pressing R

public class GameLogic : GameLogicBase
{
    private const int Rows = 20;
    private const int Columns = 10;
    private Color[,] grid;
    private Tetrimino currentPiece;
    private Random random;
    private Tetrimino nextPiece;
    private int totalLinesCleared = 0; // Track the total lines cleared


    private int tickCounter = 0;
    private const int moveThreshold = 3; //Slowing down the time

    public GameLogic(IGameView view) : base(view)
    {
        grid = new Color[Columns, Rows];
        random = new Random();

        SpawnNewPiece();

    }


    private void SpawnNewPiece()
    {
        if (nextPiece == null) // First-time game start
        {
            nextPiece = new Tetrimino(random.Next(7));
        }

        currentPiece = nextPiece; // Use the stored piece
        nextPiece = new Tetrimino(random.Next(7)); // Generate a new next piece

        // Check if the newly spawned piece immediately collides with existing blocks
        if (IsColliding(currentPiece, 0, 0))
        {
            GameOver(); // Call game over logic
            return;
        }

        DrawPiece(currentPiece, true); // Draw new piece

        // Display next piece in the "Next" preview area
        DisplayNextPiece();
    }


    private bool isGameOver = false; // Track game over state

    private void GameOver()
    {

        isGameOver = true; // Set the game state to over

        // Prevent further movement and updates
        StopGameLoop();
    }

    private void StopGameLoop()
    {
        isGameOver = true;

    }


    private void DisplayNextPiece()
    {
        if (isGameOver) return; // Stop updating next piece on game over

        if (nextPiece == null) return;

        // Clear the preview area before drawing the new piece
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                View.SetPixelHelpArea(x, y, Color.Black); // Clear the area
            }
        }

        // Draw the next piece in the NEXT area
        foreach (var block in nextPiece.GetBlocks())
        {
            int x = block.X;
            int y = block.Y;
            View.SetPixelHelpArea(x, y, nextPiece.Color);
        }
    }


    protected override void OnTimerTick()
    {
        if (isGameOver) return;

        tickCounter++;

        if (tickCounter >= moveThreshold)
        {
            MovePiece(0, 1);
            tickCounter = 0; // Reset counter after moving
        }
    }


    protected override void OnKeyPressed(Keys key)
    {
        if (key == Keys.R)
        {
            RestartGame(); // Restart the game when "R" is pressed
            return;
        }

        if (isGameOver) return; // Ignore input after game over


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

    private void ClearBoard()
    {

        // Clear the game grid in memory
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                grid[x, y] = default(Color); // Reset each cell in the grid
                View.SetPixelMainArea(x, y, Color.Black); // Clear UI pixels
            }
        }
    }


    public void RestartGame()
    {

        isGameOver = false; // Reset game-over state

        ClearBoard(); // 🔹 Ensure UI is cleared when restarting

        View.Score = 0; // Reset the score
        totalLinesCleared = 0; // Reset the lines count
        View.Lines = 0;

        nextPiece = null;
        currentPiece = null;

        SpawnNewPiece(); // Start a new game
    }





    private bool IsColliding(Tetrimino piece, int dx, int dy)
    {
        foreach (var block in piece.GetBlocks())
        {
            int x = block.X + piece.Position.X + dx;
            int y = block.Y + piece.Position.Y + dy;

            if (x < 0 || x >= Columns || y < 0 || y >= Rows || grid[x, y] != Color.Empty)

                return true;

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

            LockPiece();
        }
    }

    private void RotatePiece()
    {

        currentPiece.Rotate();
        if (IsColliding(currentPiece, 0, 0))
        {

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
        int linesCleared = 0;

        for (int y = 0; y < Rows; y++) // Iterate over each row
        {
            bool fullLine = true;
            for (int x = 0; x < Columns; x++) // Check all columns in the row
            {
                if (grid[x, y] == default(Color)) // Instead of Color.Empty, use default(Color)
                {
                    fullLine = false;
                    break;
                }
            }

            if (fullLine)
            {
                ShiftLinesDown(y);
                linesCleared++;
            }
        }
        // Increase score based on how many lines were cleared
        if (linesCleared > 0)
        {
            totalLinesCleared += linesCleared;
            View.Lines = totalLinesCleared;
            int[] lineScores = { 0, 100, 300, 500, 800 }; // Scoring system
            View.Score += lineScores[Math.Min(linesCleared, 4)];
        }
        RedrawGrid();

    }

    private void RedrawGrid()
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                View.SetPixelMainArea(x, y, grid[x, y]); // Update UI for each block
            }
        }
    }


    private void ShiftLinesDown(int clearedRow)
    {
        for (int y = clearedRow; y > 0; y--) // Start from cleared row and move upwards
        {
            for (int x = 0; x < Columns; x++)
            {
                grid[x, y] = grid[x, y - 1]; // Move row above down
            }
        }

        //Clear the top row to prevent residual blocks
        for (int x = 0; x < Columns; x++)
        {
            grid[x, 0] = default(Color); // Reset the top row
            View.SetPixelMainArea(x, 0, Color.Black);
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
