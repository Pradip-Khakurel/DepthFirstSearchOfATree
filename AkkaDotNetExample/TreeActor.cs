using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class TreeActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        private readonly IActorRef _root;
        private int _addingNodes = 0;
        private int _visitingNodes = 0;
        private HashSet<string> _exisitingNodes = new HashSet<string>();

        public TreeActor(ICustomActorFactory rootFactory)
        {
            _root = rootFactory.Create(Context);
            _exisitingNodes.Add(rootFactory.ActorName);

            NormalBehavior();
        }

        public TreeActor(string rootName) : this(new NodeActorFactory(rootName))
        {
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
            CheckAddRequest(request);

            if (_visitingNodes == 0)
            {
                _exisitingNodes.Add(request.ChildFactory.ActorName);
                _root.Tell(request);
                _addingNodes = _addingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void VisitRequestHandler(NodeActor.VisitRequest request)
        {

            if (_addingNodes == 0)
            {
                _root.Tell(new NodeActor.VisitRequest(Self, _root, null));
                _visitingNodes = _visitingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void AddResultHandler(NodeActor.AddResult result)
        {
            _addingNodes = _addingNodes - 1;

            if (_addingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        private void VisitResultHandler(NodeActor.VisitResult result)
        {
            _visitingNodes = _visitingNodes - 1;

            if (_visitingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        #endregion handlers

        #region helpers

        private void CheckAddRequest(NodeActor.AddRequest request)
        {
            if (_exisitingNodes.Contains(request.ParentName) == false)
            {
                throw new Exception($"Node {request.ParentName} does not exist");
            }
            else if (_exisitingNodes.Contains(request.ChildFactory.ActorName) == true)
            {
                throw new Exception($"Node {request.ChildFactory.ActorName} already exists!");
            }
        }

        #endregion helpers
    } // end of TreeActorClass
}
