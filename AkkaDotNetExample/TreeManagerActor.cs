using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class TreeManagerActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        private readonly IActorRef root;
        private readonly ActorSystem system;
        private int addingNodes = 0;
        private int visitingNodes = 0;

        public TreeManagerActor(ActorSystem system, string rootNodeName)
        {
            this.system = system;
            root = system.ActorOf(NodeActor.Props(system, rootNodeName), rootNodeName);

            NormalBehavior();
        }

        #region behaviors

        public void NormalBehavior()
        {
            Receive<NodeActor.AddMessage>(m => AddNodeHandler(m));

            Receive<NodeActor.VisitMessage>(m => VisitNodeHandler(m));

            Receive<NodeActor.AddCompletedMessage>(m => AddNodeCompletedHandler(m));

            Receive<NodeActor.VisitCompletedMessage>(m => VisitNodeCompletedHandler(m));
        }

        private void BusyBehavior()
        {
            Receive<NodeActor.AddMessage>(m => Stash.Stash());

            Receive<NodeActor.VisitMessage>(m => Stash.Stash());

            Receive<NodeActor.AddCompletedMessage>(m => AddNodeCompletedHandler(m));

            Receive<NodeActor.VisitCompletedMessage>(m => VisitNodeCompletedHandler(m));
        }

        #endregion behaviors

        #region handlers

        private void AddNodeHandler(NodeActor.AddMessage msg)
        {
            if(visitingNodes == 0)
            {
                root.Tell(msg);
                addingNodes = addingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void VisitNodeHandler(NodeActor.VisitMessage msg)
        {

            if (addingNodes == 0)
            {
                root.Tell(new NodeActor.VisitMessage(Self, root));
                visitingNodes = visitingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void AddNodeCompletedHandler(NodeActor.AddCompletedMessage msg)
        {
            addingNodes = addingNodes - 1;

            if (addingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        private void VisitNodeCompletedHandler(NodeActor.VisitCompletedMessage msg)
        {
            visitingNodes = visitingNodes - 1;

            if (visitingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        #endregion handlers

        public static Props Props(ActorSystem system, string rootNodeName)
        {
            return Akka.Actor.Props.Create(() => new TreeManagerActor((system), rootNodeName));
        }
    }
}
