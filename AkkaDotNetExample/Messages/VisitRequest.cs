using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{

    /// <summary>
    /// Message sent to visit the tree (when sent to the tree) or a node of the tree (when sent to a node)
    /// </summary>
    public class VisitRequest
    {
        public IActorRef Sender { get; }

        public IActorRef Recipient { get; }

        public VisitRequest Previous { get; }

        public VisitRequest(IActorRef sender, IActorRef recipient, VisitRequest previous)
        {
            Previous = previous;
            Sender = sender;
            Recipient = recipient;
        }

        public VisitRequest() : this(null, null, null)
        {
        }
    }
}
