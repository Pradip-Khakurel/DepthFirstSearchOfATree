using Akka.Actor;
using Akka.TestKit.NUnit3;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using NUnit.Framework;

namespace DepthFirstSearchOfATree.Tests
{
    [TestFixture]
    public class AkkaDotNetExampleTest : TestKit
    {
        [Test]
        public void TreeManager_should_forward_visit_request()
        {
            var manager = Sys.ActorOf(Props.Create(() => new TreeManagerActor(Sys, "rootNodeName")));

            manager.Tell(new NodeActor.VisitRequest());

            var result = ExpectMsg<NodeActor.VisitRequest>();
        }
    }
}
