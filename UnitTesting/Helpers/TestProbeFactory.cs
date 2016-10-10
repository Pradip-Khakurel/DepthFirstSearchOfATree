using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit3;
using DepthFirstSearchOfATree.AkkaDotNetExample;

namespace DepthFirstSearchOfATree.UnitTesting
{
    public class TestProbeFactory : TestKit, ICustomActorFactory
    {
        public string ActorName { get; }

        public TestProbe Probe { get; }

        public TestProbeFactory(string nodeName)
        {
            ActorName = nodeName;
            Probe = CreateTestProbe(nodeName);
        }

        public IActorRef Create(IActorRefFactory refFactory)
        {
            return Probe;
        }
    }
}
