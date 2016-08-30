using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class NodeActorFactory
    {
        public IActorRef Create(IActorRefFactory factory, string rootNodeName)
        {
            return factory.ActorOf(NodeActor.Props(rootNodeName));
        }
    }
}
