using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveNow.Utils
{
    public abstract class CompositeBase<T> : IEnumerable<T>
    {
        protected List<T> List { get; }

        protected CompositeBase(IEnumerable<T> items)
        {
            List = new List<T>(items);
        }

        protected CompositeBase(params T[] items)
            : this(items.AsEnumerable())
        { }

        protected CompositeBase(T item)
            : this(new T[] { item })
        {
            List.Add(item);
        }

        protected CompositeBase()
            : this(Enumerable.Empty<T>())
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (!List.Contains(item))
            {
                List.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            if (List.Contains(item))
            {
                List.Remove(item);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            List.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)List).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
