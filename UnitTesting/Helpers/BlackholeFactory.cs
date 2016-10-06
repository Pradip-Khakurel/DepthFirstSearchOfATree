using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using DepthFirstSearchOfATree.AkkaDotNetExample;

namespace DepthFirstSearchOfATree.UnitTesting
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
