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
            Receive<NodeActor.AddRequest>(m => AddHandler(m));

            Receive<NodeActor.VisitRequest>(m => VisitHandler(m));

            Receive<NodeActor.AddResult>(m => AddCompletedHandler(m));

            Receive<NodeActor.VisitResult>(m => VisitSuccessdHandler(m));
        }

        private void BusyBehavior()
        {
            Receive<NodeActor.AddRequest>(m => Stash.Stash());

            Receive<NodeActor.VisitRequest>(m => Stash.Stash());

            Receive<NodeActor.AddResult>(m => AddCompletedHandler(m));

            Receive<NodeActor.VisitResult>(m => VisitSuccessdHandler(m));
        }

        #endregion behaviors

        #region handlers

        private void AddHandler(NodeActor.AddRequest request)
        {
            if(visitingNodes == 0)
            {
                root.Tell(request);
                addingNodes = addingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void VisitHandler(NodeActor.VisitRequest request)
        {

            if (addingNodes == 0)
            {
                root.Tell(new NodeActor.VisitRequest(Self, root, null));
                visitingNodes = visitingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void AddCompletedHandler(NodeActor.AddResult success)
        {
            addingNodes = addingNodes - 1;

            if (addingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        private void VisitSuccessdHandler(NodeActor.VisitResult sucsess)
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
