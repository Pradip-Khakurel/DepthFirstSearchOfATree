using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaDotNetExample
{
    public class NodeActor : ReceiveActor
    {
        #region messages
        public class AddNodeMessage
        {
            public string ChildName { get; }
            public string ParentName { get; }
            public IActorRef Sender { get; }

            public AddNodeMessage(string childName, string parentName, IActorRef sender)
            {
                ChildName = childName;
                ParentName = parentName;
                Sender = sender;
            }
        }

        public class VisitNodeMessage
        {
            public int VisitingNodeIndex { get; }

            public VisitNodeMessage(int visitingNodeIndex)
            {
                VisitingNodeIndex = visitingNodeIndex;
            }
        }

        public class VisitNodeCompletedMessage
        {
            public VisitNodeMessage VisitMessage { get; }

            public VisitNodeCompletedMessage(VisitNodeMessage visitMessage)
            {
                VisitMessage = visitMessage;
            }
        }

        public class AddNodeCompletedMessage { }
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
            Receive<AddNodeMessage>(m => AddNodeHandler(m));

            Receive<VisitNodeMessage>(m => VisitNodeHandler(m));

            Receive<VisitNodeCompletedMessage>(m => VisitNodeCompletedHandler(m));
        }
        #endregion behaviors

        #region handlers
        public void AddNodeHandler(AddNodeMessage msg)
        {
            if (msg.ParentName == nodeName)
            {
                Console.WriteLine($"Adding {msg.ChildName} in node {nodeName}");

                var child = system.ActorOf(Props(this.system, msg.ChildName), msg.ChildName);
                children.Add(child);

                msg.Sender.Tell(new AddNodeCompletedMessage());
            }
            else
            {
                children.ForEach(c => c.Tell(msg));
            }
        }

        private void VisitNodeHandler(VisitNodeMessage msg)
        {
            Console.WriteLine($"Visiting {nodeName}", nodeName);

            children.FirstOrDefault()?.Tell(new VisitNodeMessage(0));

            Sender.Tell(new VisitNodeCompletedMessage(msg));
        }

        private void VisitNodeCompletedHandler(VisitNodeCompletedMessage msg)
        {
            var newVisitedIndex = msg.VisitMessage.VisitingNodeIndex + 1;

            if (children.Count() > newVisitedIndex)
                children[newVisitedIndex].Tell(new VisitNodeMessage(newVisitedIndex));
        }
        #endregion

        public static Props Props(ActorSystem system, string nodeName)
        {
            return Akka.Actor.Props.Create(() => new NodeActor(system, nodeName));
        }
    }
}
