using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.RecursiveSingleThreadedExample
{
    public class TreeNode
    { 
        private List<TreeNode> children = new List<TreeNode>();

        public IEnumerable<TreeNode> Children
        {
            get { return children; }
        }

        public string NodeName { get; }

        public TreeNode(string nodeName)
        {
            NodeName = nodeName;
        }

        public void AddNode(TreeNode node)
        {
            Console.WriteLine($"Adding {node.NodeName} in node {NodeName}");

            children.Add(node);
        }
    }
}
