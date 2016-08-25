using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.RecursiveSingleThreadedExample
{
    public class TreeNode
    { 
        private List<TreeNode> _children = new List<TreeNode>();

        public IEnumerable<TreeNode> Children
        {
            get { return _children; }
        }

        public string NodeName { get; }

        public TreeNode(string nodeName)
        {
            NodeName = nodeName;
        }

        public void AddNode(TreeNode node)
        {
            Console.WriteLine($"Adding {node.NodeName} in node {NodeName}");

            _children.Add(node);
        }
    }
}
