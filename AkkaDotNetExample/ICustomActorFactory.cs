using Akka.Actor;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public interface ICustomActorFactory
    {
        string ActorName { get; }

        IActorRef Create(IActorRefFactory refFactory);
    }
}