using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PathfindingVisualizer
{
    public class Scene
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Right { get; set; } = 15;
        public int Top { get; set; } = 250;
        public int Margin { get; set; } = 3;
        public int CellSize { get; set; } = 15;
        public Cell[, ] Matrix { get; set; }

        public Scene(int width, int height)
        {
            Width = width;
            Height = height;
            GenerateCells();
        }

        public void GenerateCells()
        {
            int m = (Height - Top - 2 * Margin) / (CellSize + Margin);
            int n = (Width - Right - 2 * Margin) / (CellSize + Margin);
            if (m > 0 && n > 0)
            {
                Matrix = new Cell[m, n];
                for (int i = 0; i < m; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        int x = Right + Margin + j * (CellSize + Margin);
                        int y = Top + Margin + i * (CellSize + Margin);
                        Cell cell = new Cell(new Point(x, y), new Size(CellSize, CellSize));
                        Matrix[i, j] = cell;
                    }

                Matrix[0, 0].IsStart = true;
                Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1].IsFinish = true;
            }
            else
                Matrix = new Cell[0, 0];
        }

        public void UpdateSize(int width, int height)
        {
            Width = width;
            Height = height;
            UpdateCells();
        }

        public void UpdateCells()
        {
            int m = (Height - Top - 2 * Margin) / (CellSize + Margin);
            int n = (Width - Right - 2 * Margin) / (CellSize + Margin);
            if (m > 0 && n > 0)
            {
                Cell[,] tmp = Matrix;
                Matrix = new Cell[m, n];
                for (int i = 0; i < m; ++i)
                    for (int j = 0; j < n; ++j)
                        if (i < tmp.GetLength(0) && j < tmp.GetLength(1))
                            Matrix[i, j] = tmp[i, j];
                        else
                        {
                            int x = Right + Margin + j * (CellSize + Margin);
                            int y = Top + Margin + i * (CellSize + Margin);
                            Cell cell = new Cell(new Point(x, y), new Size(CellSize, CellSize));
                            Matrix[i, j] = cell;
                        }
            }
            else
                Matrix = new Cell[0, 0];
        }

        public void Draw(Graphics graphics)
        {
            foreach (Cell cell in Matrix)
                cell.Draw(graphics);
        }

        public void Click(Point location, bool IsLeftClick)
        {
            foreach (Cell cell in Matrix)
                if (cell.IsClicked(location))
                {
                    if (IsLeftClick)
                        cell.IsObstacle = true;
                    else 
                        cell.IsObstacle = false;
                }
        }

        public void DFS(Form form)
        {
            Graph graph = new Graph(Matrix);
            graph.DFS(form);
        }
        public void BFS(Form form)
        {
            Graph graph = new Graph(Matrix);
            graph.BFS(form);
        }

    }
}
