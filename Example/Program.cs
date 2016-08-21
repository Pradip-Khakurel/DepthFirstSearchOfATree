using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("TreeSystem");
            var treeManager = system.ActorOf(TreeManagerActor.Props(system, "rootNode"), "treeNodeManager");

            treeManager.Tell(new NodeActor.AddNodeMessage("child1", "rootNode", treeManager));
            treeManager.Tell(new NodeActor.AddNodeMessage("child2", "rootNode", treeManager));

            treeManager.Tell(new NodeActor.VisitNodeMessage(0));

            treeManager.Tell(new NodeActor.AddNodeMessage("child1ofchild1", "child1", treeManager));
            treeManager.Tell(new NodeActor.AddNodeMessage("child2ofchild1", "child1", treeManager));

            treeManager.Tell(new NodeActor.VisitNodeMessage(0));

            treeManager.Tell(new NodeActor.AddNodeMessage("child1ofchild2", "child2", treeManager));
            treeManager.Tell(new NodeActor.AddNodeMessage("child2ofchild2", "child2", treeManager));

            treeManager.Tell(new NodeActor.VisitNodeMessage(0));

            Console.ReadKey();
        }
    }
}
