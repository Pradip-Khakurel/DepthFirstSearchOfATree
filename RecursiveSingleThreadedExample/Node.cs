using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.RecursiveSingleThreadedExample
{
    public class Node
    { 
        private List<Node> _children = new List<Node>();

        public IEnumerable<Node> Children
        {
            get { return _children; }
        }

        public string NodeName { get; }

        public Node(string nodeName)
        {
            NodeName = nodeName;
        }

        public void AddNode(Node node)
        {
            Console.WriteLine($"Adding {node.NodeName} in node {NodeName}");

            _children.Add(node);
        }
    }
}
