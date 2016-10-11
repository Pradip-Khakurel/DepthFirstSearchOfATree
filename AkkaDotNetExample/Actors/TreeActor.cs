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
            Receive<AddRequest>(m => AddRequestHandler(m));

            Receive<VisitRequest>(m => VisitRequestHandler(m));

            Receive<AddResult>(m => AddResultHandler(m));

            Receive<VisitResult>(m => VisitResultHandler(m));
        }

        private void BusyBehavior()
        {
            Receive<AddRequest>(m => Stash.Stash());

            Receive<VisitRequest>(m => Stash.Stash());

            Receive<AddResult>(m => AddResultHandler(m));

            Receive<VisitResult>(m => VisitResultHandler(m));
        }

        #endregion behaviors

        #region handlers

        private void AddRequestHandler(AddRequest request)
        {
            CheckAddRequestConsistency(request);

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

        private void VisitRequestHandler(VisitRequest request)
        {

            if (_addingNodes == 0)
            {
                _root.Tell(new VisitRequest(Self, _root, null));
                _visitingNodes = _visitingNodes + 1;
            }
            else
            {
                Stash.Stash();
                Become(() => BusyBehavior());
            }
        }

        private void AddResultHandler(AddResult result)
        {
            _addingNodes = _addingNodes - 1;

            if (_addingNodes == 0)
            {
                Stash.UnstashAll();
                Become(() => NormalBehavior());
            }
        }

        private void VisitResultHandler(VisitResult result)
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

        private void CheckAddRequestConsistency(AddRequest request)
        {
            if (_exisitingNodes.Contains(request.ParentName) == false)
            {
                throw new InvalidRequestException($"Node {request.ParentName} does not exist");
            }
            else if (_exisitingNodes.Contains(request.ChildFactory.ActorName) == true)
            {
                throw new InvalidRequestException($"Node {request.ChildFactory.ActorName} already exists!");
            }
        }

        #endregion helpers
    } // end of TreeActorClass
}
