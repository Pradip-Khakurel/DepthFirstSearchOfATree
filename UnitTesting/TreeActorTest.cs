using Akka.Actor;
using Akka.TestKit.NUnit3;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using NUnit.Framework;
using System;

namespace DepthFirstSearchOfATree.UnitTesting
{
    [TestFixture]
    public class TreeActorTest : TestKit
    {
        [Test]
        public void TreeActor_should_send_an_AddRequest_to_its_root()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child", "root", tree));

            rootFactory.Probe.ExpectMsg<AddRequest>();
        }

        [Test]
        public void TreeActor_should_send_a_VisitRequest_to_its_root()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());

            rootFactory.Probe.ExpectMsg<VisitRequest>();
        }

        [Test]
        public void TreeActor_should_throw_exception_when_receiving_an_AddRequest_for_an_nonexisting_child()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child1", "root", tree));
            tree.Tell(new AddRequest("childOfChild2", "child2", tree));

            EventFilter.Exception<InvalidOperationException>().ExpectOne(() => { });
        }

        [Test]
        public void TreeActor_should_throw_exception_when_receiving_the_same_AddRequest_twice()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child", "root", tree));
            tree.Tell(new AddRequest("child", "root", tree));

            EventFilter.Exception<InvalidOperationException>().ExpectOne(() => { });
        }

        [Test]
        public void TreeActor_should_stop_sending_messages_while_busy_processing_AddRequest()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child1", "root", tree));
            tree.Tell(new VisitRequest()); // this should trigger the busy behavior of the tree
            tree.Tell(new AddRequest("child2", "root", tree)); // the tree should be still busy

            rootFactory.Probe.ExpectMsg<AddRequest>();
            rootFactory.Probe.ExpectNoMsg();
        }

        [Test]
        public void TreeActor_should_stop_sending_messages_while_busy_processing_VisitRequest()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest()); 
            tree.Tell(new AddRequest("child1", "root", tree)); // this should trigger the busy behavior of the tree
            tree.Tell(new VisitRequest());

            rootFactory.Probe.ExpectMsg<VisitRequest>();
            rootFactory.Probe.ExpectNoMsg();
        }

        [Test]
        public void TreeActor_should_process_next_message_after_receiving_AddResult_from_its_root()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");
            var probe = rootFactory.Probe;

            tree.Tell(new AddRequest("child1", "root", tree));
            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest("child2", "root", tree));

            probe.ExpectMsg<AddRequest>(m => {
                m.Tree.Tell(new AddResult());

                // tree should process the next message
                probe.ExpectMsg<VisitRequest>();
            });
        }

        [Test]
        public void TreeActor_should_process_next_message_after_receiving_VisitResult_from_its_root()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");
            var probe = rootFactory.Probe;

            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest("child1", "root", tree));
            tree.Tell(new VisitRequest());

            probe.ExpectMsg<VisitRequest>(m => {
                m.Sender.Tell(new VisitResult(m));

                // tree should process the next message
                probe.ExpectMsg<AddRequest>();
            });
        }
    }
}
