using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class NodeActor : ReceiveActor
    {
        #region messages
        /// <summary>
        /// Message passed to add a Node
        /// </summary>
        public class AddRequest
        {
            public ICustomActorFactory ChildFactory { get; }

            public string ParentName { get; }

            public IActorRef Tree { get; }

            public AddRequest(ICustomActorFactory childFactory, string parentName, IActorRef tree)
            {
                ChildFactory = childFactory;
                ParentName = parentName;
                Tree = tree;
            }

            public AddRequest(string nodeName, string parentName, IActorRef tree) 
                : this(new NodeActorFactory(nodeName), parentName, tree)
            {
                ParentName = parentName;
                Tree = tree;
            }
        }

        /// <summary>
        /// Message passed to visit a child node
        /// </summary>
        public class VisitRequest
        {   
            public IActorRef Sender { get; }

            public IActorRef Recipient { get; }

            public VisitRequest Previous { get;  }

            public VisitRequest()
            {
                Previous = null;
            }

            public VisitRequest(IActorRef sender, IActorRef recipient, VisitRequest previous)
            {
                Previous = previous;
                Sender = sender;
                Recipient = recipient;
            }
        }

        /// <summary>
        /// Message passed to a when all children of a node (and itself) has been visited
        /// </summary>
        public class VisitResult
        {
            public VisitRequest Request { get; }

            public VisitResult(VisitRequest request)
            {
                Request = request;
            }
        }

        /// <summary>
        /// Message passed when a new node has been created and added to the tree
        /// </summary>
        public class AddResult
        {
            public AddResult()
            {
            }
        }
        
        #endregion

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
