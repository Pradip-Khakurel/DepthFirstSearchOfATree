using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class NodeActorFactory : INodeActorFactory
    {
        public string NodeName { get; }

        public NodeActorFactory(string nodeName)
        {
            NodeName = nodeName;
        }

        public IActorRef Create(IActorRefFactory refFactory)
        {
            return refFactory.ActorOf(Props.Create(() => new NodeActor(NodeName)));
        }
    }
}
