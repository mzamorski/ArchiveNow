using System.Collections.Generic;
using System.Linq;

namespace ArchiveNow.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T appendix)
        {
            return items.Concat(new[] { appendix });
        }

        public static bool None<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }
    }
}
