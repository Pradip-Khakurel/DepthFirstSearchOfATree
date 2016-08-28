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

    public class ChildActor : ReceiveActor
    {
        public ChildActor()
        {
            ReceiveAny(o => Sender.Tell("hello!"));
        }
    }

    public class ParentActor : ReceiveActor
    {
        public ParentActor()
        {
            var child = Context.ActorOf(Props.Create(() => new ChildActor()));
            ReceiveAny(o => child.Forward(o));
        }
    }

    [TestFixture]
    public class ParentGreeterSpecs : TestKit
    {
        [Test]
        public void Parent_should_create_child()
        {
            // verify child has been created by sending parent a message
            // that is forwarded to child, and which child replies to sender with
            var parentProps = Props.Create(() => new ParentActor());
            var parent = ActorOfAsTestActorRef<ParentActor>(parentProps, TestActor);
            parent.Tell("this should be forwarded to the child");
            ExpectMsg("hello!");
        }
    }

    [TestFixture]
    public class AddTest : TestKit
    {
        [Test]
        public void Node_should_receive_add_request_from_Tree()
        {
            var treeProps = Props.Create(() => new TreeActor("root"));
            var tree = ActorOfAsTestActorRef<TreeActor>(treeProps, TestActor);
            
            tree.Tell(new NodeActor.AddRequest("child", "root", tree));

            ExpectMsg<NodeActor.AddResult>();
        }
    }
}
