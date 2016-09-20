using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    /// <summary>
    /// Message sent to add a Node
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

}
