using Akka.Actor;
using Akka.TestKit.Xunit2;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DepthFirstSearchOfATree.UnitTesting
{
    public class TreeActorTest : TestKit
    {
        [Fact]
        public void TreeActor_should_send_an_AddRequest_to_its_root()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child", "root", tree));

            rootFactory.Probe.ExpectMsg<AddRequest>();
        }

        [Fact]
        public void TreeActor_should_send_a_VisitRequest_to_its_root()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());

            rootFactory.Probe.ExpectMsg<VisitRequest>();
        }

        // [Fact]
        public void TreeActor_should_throw_exception_when_receiving_the_same_AddRequest_twice()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child", "root", tree));
            tree.Tell(new AddRequest("child", "root", tree));

            EventFilter.Exception<InvalidOperationException>().Expect(1, () => { });
        }

        // [Fact]
        public void TreeActor_should_throw_exception_when_receiving_an_AddRequest_for_an_nonexisting_child()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest("child1", "root", tree));
            tree.Tell(new AddRequest("childOfChild2", "child2", tree));

            EventFilter.Exception<InvalidOperationException>().Expect(1, () => { });
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
