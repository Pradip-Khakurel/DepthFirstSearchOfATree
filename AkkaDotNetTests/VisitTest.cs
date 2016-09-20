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
    public class VisitTest : TestKit
    {

        [Test]
        public void Root_should_receive_visit_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());

            var root = rootFactory.Probe;
            root.ExpectMsg<VisitRequest>();
        }

        [Test]
        public void Root_should_send_visit_result()
        {
            var root = new NodeActorFactory("root").Create(Sys);

            root.Tell(new VisitRequest());

            ExpectMsg<VisitResult>();
        }

        [Test]
        public void Child1_should_receive_visit_request()
        {
            var root = new NodeActorFactory("root").Create(Sys);
            var child1Factory = new TestProbeFactory("child1");

            root.Tell(new AddRequest(child1Factory, "root", TestActor));
            root.Tell(new VisitRequest());

            child1Factory.Probe.ExpectMsg<VisitRequest>();
        }

        [Test]
        public void Child1_and_child2_and_child3_should_receive_visit_request()
        {
            var child1Factory = new TestProbeFactory("child1");
            var child2Factory = new TestProbeFactory("child2");
            var child3Factory = new TestProbeFactory("child3");

            var root = new NodeActorFactory("root").Create(Sys);

            var child1 = child1Factory.Probe;
            var child2 = child2Factory.Probe;
            var child3 = child3Factory.Probe;

            root.Tell(new AddRequest(child1Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new AddRequest(child2Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new AddRequest(child3Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new VisitRequest(TestActor, root, null));

            child1.ExpectMsg<VisitRequest>((m, s) =>
            {
                s.Tell(new VisitResult(m));
            });

            child2.ExpectMsg<VisitRequest>((m, s) =>
            {
                s.Tell(new VisitResult(m));
            });

            child3.ExpectMsg<VisitRequest>((m, s) =>
            {
                s.Tell(new VisitResult(m));
            });
        }

        [Test]
        public void Child3_should_not_receive_visit_request()
        {
            var child1Factory = new TestProbeFactory("child1");
            var child2Factory = new BlackHoleActorFactory("child2");
            var child3Factory = new TestProbeFactory("child3");

            var root = new NodeActorFactory("root").Create(Sys);

            var child1 = child1Factory.Probe;
            var child3 = child3Factory.Probe;

            root.Tell(new AddRequest(child1Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new AddRequest(child2Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new AddRequest(child3Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new VisitRequest(TestActor, root, null));

            child1.ExpectMsg<VisitRequest>((m, s) =>
            {
                s.Tell(new VisitResult(m));
            });

            child3.ExpectNoMsg();
        }

        [Test]
        public void Child2_and_child3_should_not_receive_visit_request()
        {
            var child1Factory = new BlackHoleActorFactory("child1");
            var child2Factory = new TestProbeFactory("child2");
            var child3Factory = new TestProbeFactory("child3");

            var root = new NodeActorFactory("root").Create(Sys);

            var child2 = child2Factory.Probe;
            var child3 = child3Factory.Probe;

            root.Tell(new AddRequest(child1Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new AddRequest(child2Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new AddRequest(child3Factory, "root", TestActor));
            ExpectMsg<AddResult>();

            root.Tell(new VisitRequest(TestActor, root, null));

            child2.ExpectNoMsg();
            child3.ExpectNoMsg();
        }
    }
}
