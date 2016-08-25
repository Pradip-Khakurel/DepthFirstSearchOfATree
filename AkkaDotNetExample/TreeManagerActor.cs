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
            Receive<NodeActor.AddRequest>(m => AddRequestHandler(m));

            Receive<NodeActor.VisitRequest>(m => VisitRequestHandler(m));

            Receive<NodeActor.AddResult>(m => AddResultHandler(m));

            Receive<NodeActor.VisitResult>(m => VisitResultHandler(m));
        }

        private void BusyBehavior()
        {
            Receive<NodeActor.AddRequest>(m => Stash.Stash());

            Receive<NodeActor.VisitRequest>(m => Stash.Stash());

            Receive<NodeActor.AddResult>(m => AddResultHandler(m));

            Receive<NodeActor.VisitResult>(m => VisitResultHandler(m));
        }

        #endregion behaviors

        #region handlers

        private void AddRequestHandler(NodeActor.AddRequest request)
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

        private void VisitRequestHandler(NodeActor.VisitRequest request)
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

        private void AddResultHandler(NodeActor.AddResult result)
        {
            addingNodes = addingNodes - 1;

            if (addingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        private void VisitResultHandler(NodeActor.VisitResult result)
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
