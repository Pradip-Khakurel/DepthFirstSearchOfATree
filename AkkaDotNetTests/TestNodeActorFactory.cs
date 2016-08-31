using Akka.TestKit.NUnit3;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DepthFirstSearchOfATree.Tests
{
    public class TestNodeActorFactory : TestKit, ICustomActorFactory
    {
        public string ActorName { get; }

        public IActorRef ActorRef { get; }

        public TestNodeActorFactory(string actorName)
        {
            ActorName = actorName;
            ActorRef = Sys.ActorOf(Props.Create(() => new NodeActor(actorName)), actorName);
        }

        public IActorRef Create(IActorRefFactory refFactory)
        {
            return ActorRef;
        }
    }
}
