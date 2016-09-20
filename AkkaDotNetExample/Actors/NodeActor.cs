using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class NodeActor : ReceiveActor
    {
        private List<IActorRef> _children { get; set; } = new List<IActorRef>();
        private readonly string _nodeName = null;

        public NodeActor(string nodeName)
        {
            _nodeName = nodeName;

            NormalBehavior();
        }

        #region behaviors
        private void NormalBehavior()
        {
            Receive<AddRequest>(m => AddRequestHandler(m));

            Receive<VisitRequest>(m => VisitRequestHandler(m));

            Receive<VisitResult>(m => VisitResultHandler(m));
        }
        #endregion behaviors

        #region handlers
        public void AddRequestHandler(AddRequest request)
        {
            if (request.ParentName == _nodeName)
            {
                var childFactory = request.ChildFactory;

                Console.WriteLine($"Adding {childFactory.ActorName} in node {_nodeName}");

                var child = childFactory.Create(Context);
                _children.Add(child);

                request.Tree.Tell(new AddResult());
            }
            else
            {
                _children.ForEach(c => c.Tell(request));
            }
        }

        private void VisitRequestHandler(VisitRequest request)
        {
            Console.WriteLine($"Visiting {_nodeName}", _nodeName);

            if (_children.Count == 0)
            {
                Sender.Tell(new VisitResult(request));
            }
            else
            {
                var firstChild = _children.First();
                var thisActorRequest = new VisitRequest(Self, firstChild, request);

                firstChild.Tell(thisActorRequest);
            }
        }

        private void VisitResultHandler(VisitResult result)
        {
            var thisActorRequest = result.Request;
            var visitedChild = thisActorRequest.Recipient;
            var nextChildindex = _children.IndexOf(visitedChild) + 1;

            // the previous request is the request of the parent node
            var parentRequest = thisActorRequest.Previous;

            if (_children.Count() > nextChildindex)
            {
                var nextChild = _children[nextChildindex];
                var newRequest = new VisitRequest(Self, nextChild, parentRequest);

                nextChild.Tell(newRequest);
            }
            else
            {
                parentRequest.Sender.Tell(new VisitResult(parentRequest));
            }             
        }
        #endregion
    }
}
