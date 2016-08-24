using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class NodeActor : ReceiveActor
    {

        /// <summary>
        /// Memos about the visit of nodes
        /// </summary>
        public class VisitMemo
        {
            public IActorRef Child { get; }

            public IActorRef Parent { get; }

            public VisitMemo(IActorRef parent, IActorRef child)
            {
                Child = child;
                Parent = parent;
            }
        }

        #region messages
        /// <summary>
        /// Message passed to add a Node
        /// </summary>
        public class AddMessage
        {
            public string ChildName { get; }
            public string ParentName { get; }
            public IActorRef TreeManager { get; }

            public AddMessage(string childName, string parentName, IActorRef treeManager)
            {
                ChildName = childName;
                ParentName = parentName;
                TreeManager = treeManager;
            }
        }

        /// <summary>
        /// Message passed to visit a node
        /// </summary>
        public class VisitMessage
        {   
            public ReadOnlyStack<VisitMemo> Memos { get; }

            public VisitMessage()
            {
                Memos = new ReadOnlyStack<VisitMemo>();
            }

            public VisitMessage(IActorRef parent, IActorRef child)
            {
                var memo = new VisitMemo(parent, child);
                Memos = new ReadOnlyStack<VisitMemo>().Push(memo);
            }

            public VisitMessage(ReadOnlyStack<VisitMemo> memos)
            {
                Memos = memos;
            }
        }

        /// <summary>
        /// Message passed to a when all children of a node (and itself) has been visited
        /// </summary>
        public class VisitCompletedMessage
        {
            public VisitMessage VisitMessage { get; }

            public VisitCompletedMessage(VisitMessage visitMessage)
            {
                VisitMessage = visitMessage;
            }
        }

        /// <summary>
        /// Message passed when a new node has been created and added to the tree
        /// </summary>
        public class AddCompletedMessage { }
        
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
            Receive<AddMessage>(m => AddNodeHandler(m));

            Receive<VisitMessage>(m => VisitNodeHandler(m));

            Receive<VisitCompletedMessage>(m => VisitNodeCompletedHandler(m));
        }
        #endregion behaviors

        #region handlers
        public void AddNodeHandler(AddMessage msg)
        {
            if (msg.ParentName == nodeName)
            {
                Console.WriteLine($"Adding {msg.ChildName} in node {nodeName}");

                var child = system.ActorOf(Props(this.system, msg.ChildName), msg.ChildName);
                children.Add(child);

                msg.TreeManager.Tell(new AddCompletedMessage());
            }
            else
            {
                children.ForEach(c => c.Tell(msg));
            }
        }

        private void VisitNodeHandler(VisitMessage msg)
        {
            Console.WriteLine($"Visiting {nodeName}", nodeName);

            if (children.Count == 0)
            {
                Sender.Tell(new VisitCompletedMessage(msg));
            }
            else
            {
                var firstChild = children.First();
                var memo = new VisitMemo(Self, firstChild);
                var editedMemos = msg.Memos.Push(memo);

                firstChild.Tell(new VisitMessage(editedMemos));
            }
        }

        private void VisitNodeCompletedHandler(VisitCompletedMessage msg)
        {
            var memos = msg.VisitMessage.Memos;
            var visitedChild = memos.Peek().Child;
            var nextChildindex = children.IndexOf(visitedChild) + 1;

            // get the parent memo from the stack
            var parentMemos = memos.Pop();

            if (children.Count() > nextChildindex)
            {
                var nextChild = children[nextChildindex];
                var editedMemos = parentMemos.Push(new VisitMemo(Self, nextChild));

                nextChild.Tell(new VisitMessage(editedMemos));
            }
            else
            {
                var parentMemo = parentMemos.Peek();
                parentMemo.Parent.Tell(new VisitCompletedMessage(new VisitMessage(memos.Pop())));
            }             
        }
        #endregion

        public static Props Props(ActorSystem system, string nodeName)
        {
            return Akka.Actor.Props.Create(() => new NodeActor(system, nodeName));
        }
    }
}
