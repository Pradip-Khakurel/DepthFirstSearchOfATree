using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using DepthFirstSearchOfATree.AkkaDotNetExample;

namespace DepthFirstSearchOfATree.UnitTesting
{
    public class TestProbeFactory : ICustomActorFactory
    {
        public string ActorName { get; }

        public TestProbe Probe { get; }

        public TestProbeFactory(string nodeName, TestKit testKit)
        {
            ActorName = nodeName;
            Probe = testKit.CreateTestProbe(nodeName);
        }

        public IActorRef Create(IActorRefFactory refFactory)
        {
            return Probe;
        }
    }
}
