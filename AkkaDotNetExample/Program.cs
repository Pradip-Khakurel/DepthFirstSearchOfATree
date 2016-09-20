using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a tree and visit it using Depth first search (Akka.Net version)");

            var system = ActorSystem.Create("treeSystem");
            var tree = system.ActorOf(Props.Create(() => new TreeActor("root")), "tree");

            tree.Tell(new AddRequest("child1", "root", tree));
            tree.Tell(new AddRequest("child2", "root", tree));

            tree.Tell(new AddRequest("child1Ofchild1", "child1", tree));
            tree.Tell(new AddRequest("child2Ofchild1", "child1", tree));

            tree.Tell(new AddRequest("child1Ofchild2", "child2", tree));
            tree.Tell(new AddRequest("child2Ofchild2", "child2", tree));

            tree.Tell(new VisitRequest());

            Console.ReadKey();
        }
    }
}
