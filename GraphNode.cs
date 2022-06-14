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
        public Cell Cell { get; set; }
        public List<GraphNode> Neighbors { get; set; }

        public GraphNode(int index, Cell cell)
        {
            Index = index;
            Cell = cell;
            Neighbors = new List<GraphNode>();
        }

        public bool ContainsNeighbor(GraphNode node) {  return Neighbors.Contains(node); }
        public void AddNeighbor(GraphNode node) { Neighbors.Add(node); }
        public bool RemoveNeighbor(GraphNode node) { return Neighbors.Remove(node); }
    }
}
