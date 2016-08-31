using Akka.Actor;
using Akka.TestKit.NUnit3;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.Tests
{
    [TestFixture]
    public class AddTest : TestKit
    {
        [Test]
        public void Root_should_sent_add_result_and_receive_add_request()
        {
            var root = new TestNodeActorFactory("root").ActorRef;

            root.Tell(new NodeActor.AddRequest("child", "root", TestActor));

            ExpectMsg<NodeActor.AddResult>();
        }

        [Test]
        public void Root_should_sent_add_result_to_tree()
        {
            var root = new TestNodeActorFactory("root").ActorRef;

            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));

            ExpectMsg<NodeActor.AddResult>((m, s) =>
            {
                var test = s == root;
                return test;
            });
        }

        [Test]
        public void Child1_should_receive_add_request_from_root()
        {
            var root = new NodeActorFactory("root").Create(Sys);
            var child1Factory = new TestProbeFactory("child1");

            root.Tell(new NodeActor.AddRequest(child1Factory, "root", TestActor));
            root.Tell(new NodeActor.AddRequest("child1OfChild1", "child1", TestActor));

            var childProbe = child1Factory.Probe;

            childProbe.ExpectMsg<NodeActor.AddRequest>((m, s) =>
            {
                var test = s == root && m.ChildFactory.ActorName == "child1OfChild1";
                return test;
            });
        }

        [Test]
        public void Child1_should_sent_add_result_to_tree()
        {
            var root = new TestNodeActorFactory("root").ActorRef;
            var child1Factory = new TestNodeActorFactory("child1");

            root.Tell(new NodeActor.AddRequest(child1Factory, "root", TestActor));
            root.Tell(new NodeActor.AddRequest("child1OfChild1", "child1", TestActor));

            ExpectMsg<NodeActor.AddResult>((m, s) => s == root);
            ExpectMsg<NodeActor.AddResult>((m, s) => s == child1Factory.ActorRef);
        }

        [Test]
        public void Child1OfChild1_should_receive_add_request()
        {
            var root = new TestNodeActorFactory("root").ActorRef;
            var child1 = new TestNodeActorFactory("child1").ActorRef;
            var child1OfChild1Factory = new TestProbeFactory("child1OfChild1");

            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));
            root.Tell(new NodeActor.AddRequest("child2", "root", TestActor));
            root.Tell(new NodeActor.AddRequest(child1OfChild1Factory, "child1", TestActor));
            root.Tell(new NodeActor.AddRequest("child1OfChild2", "child2", TestActor));
            root.Tell(new NodeActor.AddRequest("childOf...", "child1OfChild1", TestActor));

            var child1OfChild1Probe = child1OfChild1Factory.Probe;

            child1OfChild1Probe.ExpectMsg<NodeActor.AddRequest>();
        }

        [Test]
        public void Child1OfChild1_should_sent_add_result_to_tree()
        {
            var root = new TestNodeActorFactory("root").ActorRef;
            var child1factory = new TestNodeActorFactory("child1");
            var child2factory = new TestNodeActorFactory("child2");
            var child1OfChild1Factory = new TestNodeActorFactory("child1OfChild1");

            root.Tell(new NodeActor.AddRequest(child1factory, "root", TestActor));
            root.Tell(new NodeActor.AddRequest(child2factory, "root", TestActor));
            root.Tell(new NodeActor.AddRequest(child1OfChild1Factory, "child1", TestActor));
            root.Tell(new NodeActor.AddRequest("childOf...", "child1OfChild1", TestActor));

            ExpectMsg<NodeActor.AddResult>((m, s) => s == root);
            ExpectMsg<NodeActor.AddResult>((m, s) => s == root);
            ExpectMsg<NodeActor.AddResult>((m, s) => s == child1factory.ActorRef);
            ExpectMsg<NodeActor.AddResult>((m, s) => s == child1OfChild1Factory.ActorRef);
        }
    }
}
