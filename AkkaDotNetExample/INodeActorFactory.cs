using Akka.Actor;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public interface INodeActorFactory
    {
        string NodeName { get; }

        IActorRef Create(IActorRefFactory refFactory);
    }
}