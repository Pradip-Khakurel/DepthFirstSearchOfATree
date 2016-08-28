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
        public void Root_should_sent_add_result_when_receiving_add_request()
        {
            var root = Sys.ActorOf(NodeActor.Props("root"), "root");

            root.Tell(new NodeActor.AddRequest("child", "root", TestActor));

            ExpectMsg<NodeActor.AddResult>();
        }

        [Test]
        public void Root_should_be_a_child_of_tree()
        {
            var tree = Sys.ActorOf(TreeActor.Props("root"), "tree");
            var expectedRoot = ActorSelection("/user/tree/root").Anchor;

            Assert.NotNull(expectedRoot);
        }

        [Test]
        public void Child1_should_be_a_child_of_root()
        {
            var root = Sys.ActorOf(NodeActor.Props("root"), "root");

            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                var expectedChild = ActorSelection("/user/root/child1").Anchor;

                Assert.NotNull(expectedChild);
            });
        }

        [Test]
        public void Child1OfChild1_should_be_a_grandChild_of_root()
        {
            var root = Sys.ActorOf(NodeActor.Props("root"), "root");
            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));

            root.Tell(new NodeActor.AddRequest("child1OfChild1", "child1", TestActor));

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                return ActorSelection("/user/root/child1/child1OfChild1").Anchor != null;
            });

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                return ActorSelection("/user/root/child1").Anchor != null;
            });
        }
    }
}
