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
            var root = new NodeActorFactory("root").Create(Sys);

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
            var root = new NodeActorFactory("root").Create(Sys);

            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                var expectedChild = ActorSelection("/user/root/child1").Anchor;

                Assert.NotNull(expectedChild);
            });
        }

        [Test]
        public void Child1OfChild1_should_be_a_child_of_child1_and_grandChild_of_root()
        {
            var root = new NodeActorFactory("root").Create(Sys);
            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));

            root.Tell(new NodeActor.AddRequest("child1OfChild1", "child1", TestActor));

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                return ActorSelection("/user/root/child1").Anchor != null;
            });

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                return ActorSelection("/user/root/child1/child1OfChild1").Anchor != null;
            });
        }


        [Test]
        public void Child1OfChild2_should_be_a_child_of_child2_and_grandChild_of_root()
        {
            var root = new NodeActorFactory("root").Create(Sys);
            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));
            root.Tell(new NodeActor.AddRequest("child1", "root", TestActor));

            root.Tell(new NodeActor.AddRequest("child1OfChild2", "child2", TestActor));

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                return ActorSelection("/user/root/child2").Anchor != null;
            });

            ExpectMsg<NodeActor.AddResult>((message, sender) =>
            {
                return ActorSelection("/user/root/child2/child1OfChild2").Anchor != null;
            });
        }
    }
}
