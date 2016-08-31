using DepthFirstSearchOfATree.AkkaDotNetExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using Akka.TestKit.TestActors;

namespace DepthFirstSearchOfATree.Tests
{
    public class BlackHoleActorFactory : TestKit, ICustomActorFactory
    {
        public string ActorName { get;  }

        public IActorRef ActorRef { get; }

        public BlackHoleActorFactory(string actorName)
        {
            ActorName = actorName;
            ActorRef = Sys.ActorOf(BlackHoleActor.Props);
        }

        public IActorRef Create(IActorRefFactory refFactory)
        {
            return ActorRef;
        }
    }
}
