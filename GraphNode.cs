using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class GraphNode
    {
        public int Index { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Cell Cell { get; set; }
        public List<GraphNode> Neighbors { get; set; }

        // for AStar
        public GraphNode Parent { get; set; }
        public float PathCost { get; set; } = 1;

        public GraphNode(int row, int column, int index, Cell cell)
        {
            Row = row;
            Column = column;
            Index = index;
            Cell = cell;
            Neighbors = new List<GraphNode>();
        }

        public bool ContainsNeighbor(GraphNode node) {  return Neighbors.Contains(node); }
        public void AddNeighbor(GraphNode node) { Neighbors.Add(node); }
        public bool RemoveNeighbor(GraphNode node) { return Neighbors.Remove(node); }
    }
}
