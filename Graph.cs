using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathfindingVisualizer
{
    public class Graph
    {
        private int CountOfNodes { get; set; }
        private List<GraphNode> AdjacencyList { get; set; }
        public GraphNode StartNode { get; set; }
        public bool FoundSolution { get; set; } = false;
        public int MaxRow { get; set; }
        public int MaxColumn { get; set; }

        private readonly Random rand = new Random();

        public Graph(Cell[,] cells)
        {
            GenerateGraph(cells);
            FindStartNode();
        }

        private void CheckNeighborhood(Cell[, ] cells, int i, int j, int x, int y)
        {
            if ((0 <= x && x < cells.GetLength(0)) && (0 <= y && y < cells.GetLength(1)))
            {
                if (cells[x, y].CurrentState != Cell.State.Obstacle)
                    AdjacencyList[cells.GetLength(1) * i + j].AddNeighbor(AdjacencyList[cells.GetLength(1) * x + y]);
            }
        }

        private void GenerateGraph(Cell[,] cells)
        {
            CountOfNodes = cells.Length;
            AdjacencyList = new List<GraphNode>();
            for (int i = 0; i < cells.GetLength(0); ++i)
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    Debug.WriteLine(cells.GetLength(1) * i + j);
                    AdjacencyList.Add(new GraphNode(i, j, cells.GetLength(1) * i + j, cells[i, j]));
                }

            for (int i = 0; i < cells.GetLength(0); ++i)
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    int x, y;
                    x = i - 1; y = j;
                    CheckNeighborhood(cells, i, j, x, y);
                    x = i; y = j + 1;
                    CheckNeighborhood(cells, i, j, x, y);
                    x = i + 1; y = j;
                    CheckNeighborhood(cells, i, j, x, y);
                    x = i; y = j - 1;
                    CheckNeighborhood(cells, i, j, x, y);
                }

            MaxRow = cells.GetLength(0);
            MaxColumn = cells.GetLength(1);
        }

        private void FindStartNode()
        {
            foreach (GraphNode node in AdjacencyList)
                if (node.Cell.CurrentState == Cell.State.Start)
                {
                    StartNode = node;

                    return;
                }

            StartNode = null;
        }

        private void Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();
        }

        private void RetrievePath(GraphNode end, Dictionary<int, int> parentNodes, Form form, ToolStripStatusLabel tssLblReport)
        {
            List<int> path = new List<int>();
            int index = end.Index;
            while (index != StartNode.Index)
            {
                path.Add(index);
                index = parentNodes[index];
            }

            // path.Reverse();

            foreach (int idx in path)
            {
                AdjacencyList[idx].Cell.IsPath = true;
                form.Invalidate();
                Wait(10);
            }

            tssLblReport.Text += string.Format(" Path length: {0}", path.Count);
        }

        public void GenerateRandomMaze(Form form, bool showAlgoritm)
        {
            // prim algorithm for generating minimal spanning tree https://en.wikipedia.org/wiki/Maze_generation_algorithm
            AdjacencyList
                .Where(x => x.Cell.CurrentState != Cell.State.Start && x.Cell.CurrentState != Cell.State.End).ToList()
                .ForEach(x => x.Cell.ChangeState(Cell.State.Obstacle));

            List<GraphNode> walls = new List<GraphNode>();
            var startingNode = StartNode.Neighbors[rand.Next(0, 2)+1];
            startingNode.Cell.ChangeState(Cell.State.Normal);
            startingNode.Neighbors
                .Where(x => x.Cell.CurrentState != Cell.State.Start).ToList()
                .ForEach(x => walls.Add(x));

            while (walls.Count > 0)
            {
                var wall = walls[rand.Next(0,walls.Count)];
                if(wall.Row == 0 || wall.Row == MaxRow-1 || wall.Column == 0 || wall.Column == MaxColumn-1)
                {
                    walls.Remove(wall);
                    continue;
                }
                var openCells = wall.Neighbors.Where(x => x.Cell.CurrentState != Cell.State.Obstacle).ToList();
                if(openCells.Count() == 1)
                {
                    wall.Cell.CurrentState = Cell.State.Normal;
                    walls.Remove(wall);
                    GraphNode nextCell = null;
                    int xDiff = openCells[0].Column - wall.Column;
                    int yDiff = openCells[0].Row - wall.Row;
                    if (xDiff < 0)
                    {
                        nextCell = wall.Neighbors.FirstOrDefault(x => x.Column > wall.Column);
                    }
                    else if (xDiff > 0)
                    {
                        nextCell = wall.Neighbors.FirstOrDefault(x => x.Column < wall.Column);
                    }
                    else if (yDiff < 0)
                    {
                        nextCell = wall.Neighbors.FirstOrDefault(x => x.Row > wall.Row);
                    }
                    else
                    {
                        nextCell = wall.Neighbors.FirstOrDefault(x => x.Cell.TopLeft.Y < wall.Cell.TopLeft.Y);
                    }
                    if (nextCell != null && nextCell.Row != 0 && nextCell.Row != MaxRow-1 && nextCell.Column != 0 && nextCell.Column != MaxColumn-1)
                    {
                        nextCell.Cell.CurrentState = Cell.State.Normal;
                        nextCell.Neighbors
                            .Where(x => x.Cell.CurrentState == Cell.State.Obstacle).ToList()
                            .ForEach(x => walls.Add(x));
                        walls.Remove(nextCell);
                    }
                }
                walls.Remove(wall);
                if (showAlgoritm)
                {
                    wall.Cell.IsHighlighted = true;
                    form.Invalidate();
                    Wait(10);
                    wall.Cell.IsHighlighted = false;
                }
            }
            // this is to make sure there is a path from start to finish every time
            var endNode = AdjacencyList.FirstOrDefault(x => x.Cell.CurrentState == Cell.State.End);
            if (endNode.Neighbors.Where(x => x.Cell.CurrentState == Cell.State.Normal).Count() == 0)
            {
                endNode.Neighbors[rand.Next(0, 1) == 1 ? 0 : 3].Cell.ChangeState(Cell.State.Normal);
            }
            form.Invalidate();
        }

        public void BFS(Form form, ToolStripStatusLabel tssLblReport)
        {
            FoundSolution = false;
            Dictionary<int, int> parentNodes = new Dictionary<int, int>();

            bool[] visited = new bool[CountOfNodes];
            for (int i = 0; i < CountOfNodes; ++i)
                visited[i] = false;

            int countOfNodesExplored = 0;

            GraphNode node = StartNode;
            tssLblReport.Text = string.Format("Count of nodes explored: {0}", countOfNodesExplored);
            visited[node.Index] = true;
            
            node.Cell.IsVisited = true;
            form.Invalidate();
            Wait(1);

            Queue<int> queue = new Queue<int>();
            queue.Enqueue(node.Index);

            while (queue.Count > 0)
            {
                GraphNode tmp = AdjacencyList[queue.Dequeue()];
                foreach (GraphNode neighbor in tmp.Neighbors)
                    if (!visited[neighbor.Index])
                    {
                        visited[neighbor.Index] = true;
                        parentNodes[neighbor.Index] = tmp.Index;
                        neighbor.Cell.IsVisited = true;
                        tssLblReport.Text = string.Format("Count of nodes explored: {0}", ++countOfNodesExplored);
                        form.Invalidate();
                        Wait(1);

                        queue.Enqueue(neighbor.Index);

                        if (neighbor.Cell.CurrentState == Cell.State.End)
                        {
                            RetrievePath(neighbor, parentNodes, form, tssLblReport);
                            FoundSolution = true;
                            return;
                        }
                    }
            }
        }

        public void DFS(Form form, ToolStripStatusLabel tssLblReport)
        {
            FoundSolution = false;
            Dictionary<int, int> parentNodes = new Dictionary<int, int>();

            bool[] visited = new bool[CountOfNodes];
            for (int i = 0; i < CountOfNodes; ++i)
                visited[i] = false;

            int countOfNodesExplored = 0;

            DFS_Recursive(StartNode, null, visited, parentNodes, form, countOfNodesExplored, tssLblReport);

        }

        private void DFS_Recursive(GraphNode node, GraphNode parent, bool[] visited, Dictionary<int, int> parentNodes, Form form, int exploredNodes, ToolStripStatusLabel tssLblReport)
        {
            if (FoundSolution || visited[node.Index])
            {
                return;
            }
            visited[node.Index] = true;
            if (parent != null)
            {
                parentNodes[node.Index] = parent.Index;
            }
            node.Cell.IsVisited = true;
            tssLblReport.Text = string.Format("Count of nodes explored: {0}", ++exploredNodes);
            form.Invalidate();
            Wait(1);
            if (node.Cell.CurrentState == Cell.State.End)
            {
                RetrievePath(node, parentNodes, form, tssLblReport);
                FoundSolution = true;
                return;
            }
            node.Neighbors
                .Where(x => !visited[x.Index]).ToList()
                .ForEach(n => DFS_Recursive(n, node, visited, parentNodes, form, exploredNodes, tssLblReport));
        }

        public void Greedy(Form form, ToolStripStatusLabel tssLblReport, int CellSize)
        {
            FoundSolution = false;
            Dictionary<int, int> parentNodes = new Dictionary<int, int>();

            int countOfNodesExplored = 0;
            bool[] visited = new bool[CountOfNodes];
            for (int i = 0; i < CountOfNodes; ++i)
                visited[i] = false;
            var endNode = AdjacencyList.FirstOrDefault(x => x.Cell.CurrentState == Cell.State.End);
            SimplePriorityQueue<GraphNode, float> queue = new SimplePriorityQueue<GraphNode, float>();
            queue.Enqueue(StartNode, 0);
            var current = StartNode;
            while (queue.Count != 0)
            {
                current = queue.Dequeue();
                current.Cell.IsVisited = true;
                visited[current.Index] = true;
                tssLblReport.Text = string.Format("Count of nodes explored: {0}", ++countOfNodesExplored);
                form.Invalidate();
                Wait(1);
                if (current.Cell.CurrentState == Cell.State.End)
                {
                    RetrievePath(current, parentNodes, form, tssLblReport);
                    FoundSolution = true;
                    return;
                }

                foreach (var n in current.Neighbors)
                {
                    if (!queue.ToList().Contains(n) && !visited[n.Index])
                    {
                        parentNodes[n.Index] = current.Index;
                        queue.Enqueue(n, (float)(Heuristic(n, endNode)));
                    }
                    
                }
            }
        }

        public void AStar(Form form, ToolStripStatusLabel tssLblReport, int CellSize)
        {
            FoundSolution = false;
            Dictionary<int, int> parentNodes = new Dictionary<int, int>();
            Dictionary<int, int> shortestPathCost = new Dictionary<int, int>();


            for (int i = 0; i < CountOfNodes; ++i)
            {
                shortestPathCost.Add(i, int.MaxValue);
            }
            shortestPathCost[StartNode.Index] = 0;
            int countOfNodesExplored = 0;

            var endNode = AdjacencyList.FirstOrDefault(x => x.Cell.CurrentState == Cell.State.End);
            SimplePriorityQueue<GraphNode, float> queue = new SimplePriorityQueue<GraphNode, float>();
            queue.Enqueue(StartNode, 0);
            var current = StartNode;
            while (queue.Count != 0)
            {
                current = queue.Dequeue();
                current.Cell.IsVisited = true;
                tssLblReport.Text = string.Format("Count of nodes explored: {0}", ++countOfNodesExplored);
                form.Invalidate();
                Wait(1);
                if (current.Cell.CurrentState == Cell.State.End)
                {
                    RetrievePath(current, parentNodes, form, tssLblReport);
                    FoundSolution = true;
                    return;
                }

                foreach (var n in current.Neighbors)
                {
                    var score = shortestPathCost[current.Index] + 1;
                    if (score < shortestPathCost[n.Index]) {
                        parentNodes[n.Index] = current.Index;
                        shortestPathCost[n.Index] = score;
                        if (queue.ToList().Contains(n))
                        {
                            queue.UpdatePriority(n, (float)(Heuristic(n, endNode) + score));
                        }
                        else
                        {
                            queue.Enqueue(n, (float)(Heuristic(n, endNode) + score));
                        }
                    }
                }
            }
        }
        private double Heuristic(GraphNode startNode, GraphNode endNode)
        {
            return Math.Abs(startNode.Row - endNode.Row) + Math.Abs(startNode.Column - endNode.Column);
        }
    }
}
