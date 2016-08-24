using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.AkkaDotNetExample
{
    public class ReadOnlyStack<T> : IEnumerable<T>
    {
        private readonly IList<T> list; // only assigned once; 

        public ReadOnlyStack()
        {
            list = new List<T>();
        }
       
        private ReadOnlyStack(IEnumerable<T> enumerable)
        {
            list = new List<T>(enumerable);
        }

        public ReadOnlyStack<T> Push(T item)
        {
            var newList= new List<T>(list);
            newList.Add(item);

            return new ReadOnlyStack<T>(newList);
        }

        public ReadOnlyStack<T> Pop()
        {
            var newList = new List<T>(list);
            newList.RemoveAt(list.Count() - 1);

            return new ReadOnlyStack<T>(newList);
        }

        public T Peek()
        {
            return list.Last();
        }

        public bool IsEmpty()
        {
            return list.Count() == 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
