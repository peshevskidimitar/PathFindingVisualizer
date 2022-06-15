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
                    AdjacencyList.Add(new GraphNode(cells.GetLength(1) * i + j, cells[i, j]));
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

            tssLblReport.Text += string.Format(" Length of the shortest path: {0}", path.Count);
        }

        public void BFS(Form form, ToolStripStatusLabel tssLblReport)
        {
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

                            return;
                        }
                    }
            }
        }
    }
}
