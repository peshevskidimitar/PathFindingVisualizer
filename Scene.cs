using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace PathfindingVisualizer
{
    [Serializable]
    public class Scene
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Right { get; set; } = 15;
        public int Top { get; set; } = 100;
        public int Margin { get; set; } = 1;
        public int CellSize { get; set; } = 15;
        public Cell[, ] Cells { get; set; }

        public bool IsStartCellSelected { get; set; }
        public bool IsEndCellSelected { get; set; }

        public Scene(int width, int height)
        {
            Width = width;
            Height = height;
            GenerateCells();
        }

        public void GenerateCells()
        {
            int m = (Height - Top - Margin) / (CellSize + Margin);
            int n = (Width - 20 - Margin) / (CellSize + Margin);
            Right = (Width - n * (CellSize + Margin)) / 2;
            if (m > 0 && n > 0)
            {
                Cells = new Cell[m, n];
                for (int i = 0; i < m; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        int x = Right + Margin + j * (CellSize + Margin);
                        int y = Top + Margin + i * (CellSize + Margin);
                        Cell cell = new Cell(new Point(x, y), new Size(CellSize, CellSize));
                        Cells[i, j] = cell;
                    }

                Cells[1, 1].ChangeState(Cell.State.Start);
                Cells[Cells.GetLength(0) - 2, Cells.GetLength(1) - 2].ChangeState(Cell.State.End);
            }
            else
                Cells = new Cell[0, 0];
        }

        public bool IsThereStartFlag()
        {
            foreach (Cell cell in Cells)
                if (cell.CurrentState == Cell.State.Start)
                    return true;

            return false;
        }

        public bool IsThereEndFlag()
        {
            foreach (Cell cell in Cells)
                if (cell.CurrentState == Cell.State.End)
                    return true;

            return false;
        }

        public void ClearStartFlags()
        {
            foreach (Cell cell in Cells)
                if (cell.CurrentState == Cell.State.Start)
                    cell.UndoState();
        }

        public void ClearEndFlags()
        {
            foreach (Cell cell in Cells)
                if (cell.CurrentState == Cell.State.End)
                    cell.UndoState();
        }

        public void UpdateSize(int width, int height)
        {
            Width = width;
            Height = height;
            UpdateCells();
        }

        public void UpdateCellSize(int size)
        {
            CellSize = size;
            foreach (Cell cell in Cells)
                cell.Size = new Size(size, size);
            GenerateCells();
        }

        private void UpdateCells()
        {
            int m = (Height - Top - Margin) / (CellSize + Margin);
            int n = (Width - 20 - Margin) / (CellSize + Margin);
            Right = (Width - n * (CellSize + Margin)) / 2;
            if (m > 0 && n > 0)
            {
                Cell[,] tmp = Cells;
                Cells = new Cell[m, n];
                for (int i = 0; i < m; ++i)
                    for (int j = 0; j < n; ++j)
                        if (i < tmp.GetLength(0) && j < tmp.GetLength(1))
                        {
                            tmp[i, j].TopLeft = new Point(Right + Margin + j * (CellSize + Margin), tmp[i, j].TopLeft.Y);
                            Cells[i, j] = tmp[i, j];
                        }
                        else
                        {
                            int x = Right + Margin + j * (CellSize + Margin);
                            int y = Top + Margin + i * (CellSize + Margin);
                            Cell cell = new Cell(new Point(x, y), new Size(CellSize, CellSize));
                            Cells[i, j] = cell;
                        }

                
                if (!IsThereStartFlag())
                    Cells[1, 1].ChangeState(Cell.State.Start);
                if (!IsThereEndFlag())
                    Cells[Cells.GetLength(0) - 2, Cells.GetLength(1) - 2].ChangeState(Cell.State.End);
            }
            else
                Cells = new Cell[0, 0];
        }

        public void Draw(Graphics graphics)
        {
            foreach (Cell cell in Cells)
                cell.Draw(graphics);
        }

        public void Click(Point location, bool IsLeftClick, Point point)
        {
            foreach (Cell cell in Cells)
                if (cell.IsClicked(location))
                {
                    
                    if (IsStartCellSelected)
                    {
                        if (cell.CurrentState != Cell.State.End)
                        {
                            ClearStartFlags();
                            cell.ChangeState(Cell.State.Start);
                        }
                    }
                    else if (IsEndCellSelected)
                    {
                        if (cell.CurrentState != Cell.State.Start)
                        {
                            ClearEndFlags();
                            cell.ChangeState(Cell.State.End);
                        }
                    }
                    else if (IsStartCellClicked(point))
                        IsStartCellSelected = true;
                    else if (IsEndCellClicked(point))
                        IsEndCellSelected = true;
                    else if (cell.CurrentState == Cell.State.Normal || cell.CurrentState == Cell.State.Obstacle)
                    {
                        if (IsLeftClick)
                            cell.ChangeState(Cell.State.Obstacle);
                        else
                            cell.ChangeState(Cell.State.Normal);
                    }
                }
        }

        public bool IsStartCellClicked(Point location)
        {
            foreach (Cell cell in Cells)
                if (cell.IsClicked(location))
                    if (cell.CurrentState == Cell.State.Start)
                        return true;

            return false;
        }

        public bool IsEndCellClicked(Point location)
        {
            foreach (Cell cell in Cells)
                if (cell.IsClicked(location))
                    if (cell.CurrentState == Cell.State.End)
                        return true;

            return false;
        }

        public void ClearVisitedAndPathFlags()
        {
            foreach (Cell cell in Cells)
                cell.IsVisited = cell.IsPath = false;
        }

        public void BFS(Form form, ToolStripStatusLabel tssLblReport)
        {
            ClearVisitedAndPathFlags();
            Graph graph = new Graph(Cells);
            graph.BFS(form, tssLblReport);
        }

        public void DFS(Form form, ToolStripStatusLabel tssLblReport)
        {
            ClearVisitedAndPathFlags();
            Graph graph = new Graph(Cells);
            graph.DFS(form, tssLblReport);
        }

        public void Greedy(Form form, ToolStripStatusLabel tssLblReport)
        {
            ClearVisitedAndPathFlags();
            Graph graph = new Graph(Cells);
            Cell endCell = null;
            foreach (Cell cell in Cells) 
            {
                if (cell.CurrentState == Cell.State.End)
                {
                    endCell = cell;
                    break;
                }
            }
            graph.Greedy(form, tssLblReport, CellSize);
        }

        public void AStar(Form form, ToolStripStatusLabel tssLblReport)
        {
            ClearVisitedAndPathFlags();
            Graph graph = new Graph(Cells);
            Cell endCell = null;
            foreach (Cell cell in Cells)
            {
                if (cell.CurrentState == Cell.State.End)
                {
                    endCell = cell;
                    break;
                }
            }
            graph.AStar(form, tssLblReport, CellSize);
        }

        public void GenerateMaze(Form form, bool showAlgorithm)
        {
            ClearVisitedAndPathFlags();
            foreach (Cell cell in Cells) {
                if (cell.CurrentState == Cell.State.Obstacle) 
                {
                    cell.ChangeState(Cell.State.Normal);
                }
            }
            Graph graph = new Graph(Cells);
            graph.GenerateRandomMaze(form, showAlgorithm);
        }

    }
}
