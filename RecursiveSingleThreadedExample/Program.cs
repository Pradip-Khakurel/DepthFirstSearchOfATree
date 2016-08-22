using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.RecursiveSingleThreadedExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new TreeNode("rootNode");

            var child1 = new TreeNode("child1");
            var child2 = new TreeNode("child2");

            var child1Ofchild1 = new TreeNode("child1Ofchild1");
            var child2Ofchild1 = new TreeNode("child2Ofchild1");

            var child1Ofchild2 = new TreeNode("child1Ofchild2");
            var child2Ofchild2 = new TreeNode("child2Ofchild2");

            root.AddNode(child1);
            root.AddNode(child2);

            Traverse(root);

            child1.AddNode(child1Ofchild1);
            child1.AddNode(child2Ofchild1);

            Traverse(root);

            child2.AddNode(child1Ofchild2);
            child2.AddNode(child2Ofchild2);

            Traverse(root);

            Console.ReadKey();
        }

        static void Traverse(TreeNode node)
        {
            Console.WriteLine($"Visiting {node.NodeName}", node.NodeName);

            foreach (var child in node.Children) Traverse(child);
        }
    
    }
}
