using Akka.TestKit.NUnit3;
using DepthFirstSearchOfATree.AkkaDotNetExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit;
using NUnit.Framework;

namespace DepthFirstSearchOfATree.Tests
{
    [TestFixture]
    public class ConcurencyTest : TestKit
    {
        #region tests_beginning_with_add_request
        [Test]
        public void Root_should_receive_add_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest(child1Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<AddRequest>();
        }

        [Test]
        public void Root_should_not_received_visit_request_after_add_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest(child1Factory, "root", tree));
            tree.Tell(new VisitRequest());

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<AddRequest>();
            rootProbe.ExpectNoMsg();
        }

        [Test]
        public void Root_should_stop_receiving_any_requests_after_add_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");
            var child2Factory = new TestProbeFactory("child2");
            var child3Factory = new TestProbeFactory("child3");
            var child4Factory = new TestProbeFactory("child4");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest(child1Factory, "root", tree));
            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child2Factory, "root", tree));
            tree.Tell(new AddRequest(child3Factory, "root", tree));
            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child4Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<AddRequest>();
            rootProbe.ExpectNoMsg();
        }

        [Test]
        public void Root_should_received_visit_request_after_add_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

 
            tree.Tell(new AddRequest(child1Factory, "root", tree));
            tree.Tell(new VisitRequest());

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<AddRequest>(m =>
            {
                tree.Tell(new AddResult());

                rootProbe.ExpectMsg<VisitRequest>();
            });
        }

        [Test]
        public void Root_should_receive_all_requests_after_add_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");
            var child2Factory = new TestProbeFactory("child2");
            var child3Factory = new TestProbeFactory("child3");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new AddRequest(child1Factory, "root", tree));
            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child2Factory, "root", tree));
            tree.Tell(new AddRequest(child3Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<AddRequest>(m1 =>
            {
                tree.Tell(new AddResult());

                rootProbe.ExpectMsg<VisitRequest>(m2 =>
                {
                    tree.Tell(new VisitResult(m2));

                    rootProbe.ExpectMsg<AddRequest>();
                    rootProbe.ExpectMsg<AddRequest>();
                });
            });
        }
        #endregion tests_beginning_with_add_request

        #region tests_beginning_with_visit_request
        [Test]
        public void Root_should_receive_visit_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<VisitRequest>();
        }

        [Test]
        public void Root_should_not_received_add_request_after_visit_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child1Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<VisitRequest>();
            rootProbe.ExpectNoMsg();
        }

        [Test]
        public void Root_should_stop_receiving_any_requests_after_visit_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");
            var child2Factory = new TestProbeFactory("child2");
            var child3Factory = new TestProbeFactory("child3");
            var child4Factory = new TestProbeFactory("child4");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child1Factory, "root", tree));
            tree.Tell(new AddRequest(child2Factory, "root", tree));
            tree.Tell(new AddRequest(child3Factory, "root", tree));
            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child4Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<VisitRequest>();
            rootProbe.ExpectNoMsg();
        }

        [Test]
        public void Root_should_received_add_request_after_visit_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child1Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<VisitRequest>(m =>
            {
                tree.Tell(new VisitResult(m));

                rootProbe.ExpectMsg<AddRequest>();
            });
        }

        [Test]
        public void Root_should_receive_all_requests_after_visit_request()
        {
            var rootFactory = new TestProbeFactory("root");
            var child1Factory = new TestProbeFactory("child1");
            var child2Factory = new TestProbeFactory("child2");
            var child3Factory = new TestProbeFactory("child3");

            var tree = Sys.ActorOf(Props.Create(() => new TreeActor(rootFactory)), "tree");

            tree.Tell(new VisitRequest());
            tree.Tell(new AddRequest(child1Factory, "root", tree));
            tree.Tell(new AddRequest(child2Factory, "root", tree));
            tree.Tell(new AddRequest(child3Factory, "root", tree));

            var rootProbe = rootFactory.Probe;

            rootProbe.ExpectMsg<VisitRequest>(m1 =>
            {
                tree.Tell(new VisitResult(m1));

                rootProbe.ExpectMsg<AddRequest>();
                rootProbe.ExpectMsg<AddRequest>();
                rootProbe.ExpectMsg<AddRequest>();
            });
        }
        #endregion tests_beginning_with_add_request
    }
}