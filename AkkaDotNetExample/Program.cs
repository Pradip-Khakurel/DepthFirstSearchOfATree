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
            var system = ActorSystem.Create("treeSystem");
            var tree = system.ActorOf(TreeActor.Props("root"), "tree");

            tree.Tell(new NodeActor.AddRequest("child1", "root", tree));
            tree.Tell(new NodeActor.AddRequest("child2", "root", tree));

            tree.Tell(new NodeActor.VisitRequest());

            tree.Tell(new NodeActor.AddRequest("child1Ofchild1", "child1", tree));
            tree.Tell(new NodeActor.AddRequest("child2Ofchild1", "child1", tree));

            tree.Tell(new NodeActor.VisitRequest());

            tree.Tell(new NodeActor.AddRequest("child1Ofchild2", "child2", tree));
            tree.Tell(new NodeActor.AddRequest("child2Ofchild2", "child2", tree));

            tree.Tell(new NodeActor.VisitRequest());

            Console.ReadKey();
        }
    }
}
