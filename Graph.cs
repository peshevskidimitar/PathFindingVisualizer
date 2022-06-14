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
        public GraphNode Start { get; set; }

        public Graph(Cell[,] cells)
        {
            GenerateGraph(cells);
            foreach (GraphNode node in AdjacencyList)
                if (node.Cell.IsStart == true)
                {
                    Start = node;
                    break;
                }
        }

        private void CheckNeighborhood(Cell[, ] cells, int i, int j, int x, int y)
        {
            if ((0 <= x && x < cells.GetLength(0)) && (0 <= y && y < cells.GetLength(1)))
            {
                if (!cells[x, y].IsObstacle)
                    AdjacencyList[cells.GetLength(1) * i + j].AddNeighbor(AdjacencyList[cells.GetLength(1) * x + y]);
            }
        }

        public void GenerateGraph(Cell[,] cells)
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

        public void Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();
        }


        public void DFS(Form form)
        {
            Dictionary<int, int> parentNodes = new Dictionary<int, int>();

            bool[] visited = new bool[CountOfNodes];
            for (int i = 0; i < CountOfNodes; ++i)
                visited[i] = false;

            GraphNode node = Start;

            visited[node.Index] = true;
            node.Cell.IsVisited = true;
            form.Invalidate();
            Wait(50);

            Stack<int> stack = new Stack<int>();
            stack.Push(node.Index);

            while (stack.Count > 0)
            {
                GraphNode tmp = AdjacencyList[stack.Peek()];

                GraphNode nextNode = null;
                foreach (GraphNode neighbor in tmp.Neighbors)
                {
                    nextNode = neighbor;

                    if (!visited[nextNode.Index])
                        break;
                }

                if (nextNode != null && !visited[nextNode.Index])
                {
                    visited[nextNode.Index] = true;
                    parentNodes[nextNode.Index] = tmp.Index;
                    nextNode.Cell.IsVisited = true;
                    form.Invalidate();
                    Wait(50);
                    if (nextNode.Cell.IsFinish)
                    {
                        List<int> shortestPath = new List<int>();
                        int index = nextNode.Index;
                        while (index != Start.Index)
                        {
                            shortestPath.Add(index);
                            index = parentNodes[index];
                        }

                        // shortestPath.Reverse();

                        foreach (int idx in shortestPath)
                        {
                            AdjacencyList[idx].Cell.IsPath = true;
                            form.Invalidate();
                            Wait(10);
                        }

                        return;
                    }
                    stack.Push(nextNode.Index);
                }
                else
                    stack.Pop();
            }
        }

        public void BFS(Form form)
        {
            Dictionary<int, int> parentNodes = new Dictionary<int, int>();

            bool[] visited = new bool[CountOfNodes];
            for (int i = 0; i < CountOfNodes; ++i)
                visited[i] = false;

            GraphNode node = Start;

            visited[node.Index] = true;
            node.Cell.IsVisited = true;
            form.Invalidate();
            Wait(10);

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
                        form.Invalidate();
                        Wait(10);

                        queue.Enqueue(neighbor.Index);

                        if (neighbor.Cell.IsFinish)
                        {
                            List<int> shortestPath = new List<int>();
                            int index = neighbor.Index;
                            while (index != Start.Index)
                            {
                                shortestPath.Add(index);
                                index = parentNodes[index];
                            }

                            // shortestPath.Reverse();

                            foreach (int idx in shortestPath)
                            {
                                AdjacencyList[idx].Cell.IsPath = true;
                                form.Invalidate();
                                Wait(10);
                            }

                            return;
                        }
                    }
            }
        }
    }
}
