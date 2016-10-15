﻿using Akka.Actor;
using Akka.TestKit.Xunit2;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using Xunit;

namespace DepthFirstSearchOfATree.UnitTesting
{
    public class NodeActorTest : TestKit
    {
        [Fact]
        public void NodeActor_should_send_AddResult_to_the_TreeActor_when_the_message_is_for_him()
        {
            var nodeActor = Sys.ActorOf(Props.Create(() => new NodeActor("node")));
            var tree = CreateTestProbe("tree");

            // adding child
            nodeActor.Tell(new AddRequest("child", "node", tree));

            tree.ExpectMsg<AddResult>((m, s) => s == nodeActor);
        }

        [Fact]
        public void NodeActor_should_not_send_AddResult_to_the_TreeActor_when_the_message_is_not_for_him()
        {
            var nodeActor = Sys.ActorOf(Props.Create(() => new NodeActor("node")));
            var tree = CreateTestProbe("tree");

            // adding to an other node
            nodeActor.Tell(new AddRequest("child", "otherNode", tree));

            tree.ExpectNoMsg();
        }

        [Fact]
        public void NodeActor_should_always_send_VisitRequest_to_its_child()
        {
            var nodeActor = Sys.ActorOf(Props.Create(() => new NodeActor("node")));
            var child1Factory = new TestProbeFactory("child1", this);
            var tree = CreateTestProbe("tree");

            // adding child1
            nodeActor.Tell(new AddRequest(child1Factory, "node", tree));
            tree.ExpectMsg<AddResult>();

            nodeActor.Tell(new VisitRequest(tree, nodeActor, null));

            var child1 = child1Factory.Probe;

            child1.ExpectMsg<VisitRequest>();
        }

        [Fact]
        public void NodeActor_not_should_send_VisitRequest_to_its_second_child_without_receiving_VisitResult_from_the_first_one()
        {
            var nodeActor = Sys.ActorOf(Props.Create(() => new NodeActor("node")));
            var child1Factory = new TestProbeFactory("child1", this);
            var child2Factory = new TestProbeFactory("child2", this);
            var tree = CreateTestProbe("tree");

            // adding child1
            nodeActor.Tell(new AddRequest(child1Factory, "node", this.TestActor));

            // adding child2
            nodeActor.Tell(new AddRequest(child2Factory, "node", this.TestActor));

            nodeActor.Tell(new VisitRequest(this.TestActor, nodeActor, null));

            var child2 = child2Factory.Probe;

            child2.ExpectNoMsg();
        }

        [Fact]
        public void NodeActor_should_send_VisitRequest_to_the_second_child_when_receiving_VisitResult_from_the_first_one()
        {
            var nodeActor = Sys.ActorOf(Props.Create(() => new NodeActor("node")));
            var child1Factory = new TestProbeFactory("child1", this);
            var child2Factory = new TestProbeFactory("child2", this);
            var tree = CreateTestProbe("tree");

            // adding child1
            nodeActor.Tell(new AddRequest(child1Factory, "node", tree));
            tree.ExpectMsg<AddResult>();

            // adding child2
            nodeActor.Tell(new AddRequest(child2Factory, "node", tree));
            tree.ExpectMsg<AddResult>();

            nodeActor.Tell(new VisitRequest(tree, nodeActor, null));

            var child1 = child1Factory.Probe;
            var child2 = child2Factory.Probe;

            child1.ExpectMsg<VisitRequest>(m => {
                m.Sender.Tell(new VisitResult(m)); // sending VisitResult after receiving VisitRequest
            });

            child2.ExpectMsg<VisitRequest>();
        }

        [Fact]
        public void NodeActor_should_send_VisitResult_to_its_parent_after_receiving_VisitRequest_from_its_last_child()
        {
            var nodeActor = Sys.ActorOf(Props.Create(() => new NodeActor("node")));
            var child1Factory = new TestProbeFactory("child1", this);
            var child2Factory = new TestProbeFactory("child2", this);
            var tree = CreateTestProbe("tree");

            // adding child1 - TestActor is the parent of NodeActor
            nodeActor.Tell(new AddRequest(child1Factory, "node", tree));
            
            // adding child2
            nodeActor.Tell(new AddRequest(child2Factory, "node", tree));

            tree.ExpectMsg<AddResult>(); // first AddRequest
            tree.ExpectMsg<AddResult>(); // second AddRequest

            nodeActor.Tell(new VisitRequest(tree, nodeActor, null));

            var child1 = child1Factory.Probe;
            var child2 = child2Factory.Probe;

            child1.ExpectMsg<VisitRequest>(m => { // child1 receives the VisitRequest first ...
                m.Sender.Tell(new VisitResult(m));
            });

            child2.ExpectMsg<VisitRequest>(m => { // then child2 receives the VisitRequest ...
                m.Sender.Tell(new VisitResult(m));  
            });

            tree.ExpectMsg<VisitResult>(); // and then NodeActor sends back VisitResult
        }
    }
}
