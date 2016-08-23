using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public static class StackExtension
    {
        public static Stack<T> ToStack<T>(this IEnumerable<T> collection)
        {
            return new Stack<T>(collection);
        }
    }
}
