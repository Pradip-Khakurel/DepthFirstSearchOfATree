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
            var treeManager = system.ActorOf(TreeManagerActor.Props(system, "rootNode"), "treeNodeManager");

            treeManager.Tell(new NodeActor.AddNodeMessage("child1", "rootNode", treeManager));
            treeManager.Tell(new NodeActor.AddNodeMessage("child2", "rootNode", treeManager));

            treeManager.Tell(new NodeActor.VisitNodeMessage());

            treeManager.Tell(new NodeActor.AddNodeMessage("child1Ofchild1", "child1", treeManager));
            treeManager.Tell(new NodeActor.AddNodeMessage("child2Ofchild1", "child1", treeManager));

            treeManager.Tell(new NodeActor.VisitNodeMessage());

            treeManager.Tell(new NodeActor.AddNodeMessage("child1Ofchild2", "child2", treeManager));
            treeManager.Tell(new NodeActor.AddNodeMessage("child2Ofchild2", "child2", treeManager));

            treeManager.Tell(new NodeActor.VisitNodeMessage());

            Console.ReadKey();
        }
    }
}
