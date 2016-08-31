using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class NodeActorFactory : ICustomActorFactory
    {
        public string ActorName { get; }

        public NodeActorFactory(string nodeName)
        {
            ActorName = nodeName;
        }

        public IActorRef Create(IActorRefFactory refFactory)
        {
            return refFactory.ActorOf(Props.Create(() => new NodeActor(ActorName)));
        }
    }
}
