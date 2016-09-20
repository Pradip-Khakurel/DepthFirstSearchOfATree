using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    /// <summary>
    ///  Message sent after all children of a node (and itself) has been visited
    /// </summary>
    public class VisitResult
    {
        public VisitRequest Request { get; }

        public VisitResult(VisitRequest request)
        {
            Request = request;
        }
    }
}
