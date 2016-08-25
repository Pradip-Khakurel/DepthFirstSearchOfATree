using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

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
            public string ChildName { get; }
            public string ParentName { get; }
            public IActorRef TreeManager { get; }

            public AddRequest(string childName, string parentName, IActorRef treeManager)
            {
                ChildName = childName;
                ParentName = parentName;
                TreeManager = treeManager;
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
        public class AddResult { }
        
        #endregion

        private List<IActorRef> children { get; set; } = new List<IActorRef>();
        private readonly ActorSystem system = null;
        private readonly string nodeName = null;

        public NodeActor(ActorSystem system, string nodeName)
        {
            this.nodeName = nodeName;
            this.system = system;

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
            if (request.ParentName == nodeName)
            {
                Console.WriteLine($"Adding {request.ChildName} in node {nodeName}");

                var child = system.ActorOf(Props(this.system, request.ChildName), request.ChildName);
                children.Add(child);

                request.TreeManager.Tell(new AddResult());
            }
            else
            {
                children.ForEach(c => c.Tell(request));
            }
        }

        private void VisitRequestHandler(VisitRequest request)
        {
            Console.WriteLine($"Visiting {nodeName}", nodeName);

            if (children.Count == 0)
            {
                Sender.Tell(new VisitResult(request));
            }
            else
            {
                var firstChild = children.First();
                var thisActorRequest = new VisitRequest(Self, firstChild, request);

                firstChild.Tell(thisActorRequest);
            }
        }

        private void VisitResultHandler(VisitResult result)
        {
            var thisActorRequest = result.Request;
            var visitedChild = thisActorRequest.Recipient;
            var nextChildindex = children.IndexOf(visitedChild) + 1;

            // the previous request is the request of the parent node
            var parentRequest = thisActorRequest.Previous;

            if (children.Count() > nextChildindex)
            {
                var nextChild = children[nextChildindex];
                var newRequest = new VisitRequest(Self, nextChild, parentRequest);

                nextChild.Tell(newRequest);
            }
            else
            {
                parentRequest.Sender.Tell(new VisitResult(parentRequest));
            }             
        }
        #endregion

        public static Props Props(ActorSystem system, string nodeName)
        {
            return Akka.Actor.Props.Create(() => new NodeActor(system, nodeName));
        }
    }
}
