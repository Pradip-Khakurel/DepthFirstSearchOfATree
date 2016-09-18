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
            Console.WriteLine("Create a tree and visit it using Depth first search (recursive algorithm single threaded version)");

            var root = new Node("root");

            var child1 = new Node("child1");
            var child2 = new Node("child2");

            var child1Ofchild1 = new Node("child1Ofchild1");
            var child2Ofchild1 = new Node("child2Ofchild1");

            var child1Ofchild2 = new Node("child1Ofchild2");
            var child2Ofchild2 = new Node("child2Ofchild2");

            root.AddNode(child1);
            root.AddNode(child2);

            child1.AddNode(child1Ofchild1);
            child1.AddNode(child2Ofchild1);

            child2.AddNode(child1Ofchild2);
            child2.AddNode(child2Ofchild2);

            DepthFirstSearch(root);

            Console.ReadKey();
        }

        static void DepthFirstSearch(Node node)
        {
            Console.WriteLine($"Visiting {node.NodeName}", node.NodeName);

            foreach (var child in node.Children) DepthFirstSearch(child);
        }
    
    }
}
