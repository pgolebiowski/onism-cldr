using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onism.Cldr.Extensions
{
    internal static class ReadOnlyListExtensions
    {
        /// <summary>
        /// Aggregates a sequence "top-down", in a hierarchical fashion. When aggregating complex objects,
        /// this approach is more efficient, since each object will be merged at most O(log n) times.
        /// </summary>
        /// <remarks>
        /// For efficiency this requires random access to the source sequence so building
        /// on top on <see cref="IEnumerable{T}"/> is not the best choice.
        /// </remarks>
        public static T HierarchicalAggregate<T>(this IReadOnlyList<T> source, Func<T, T, T> func)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (source.Count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            return Recurse(source, 0, source.Count, func);
        }

        private static T Recurse<T>(IReadOnlyList<T> source, int startIndex, int count, Func<T, T, T> func)
        {
            if (count == 1)
                return source[startIndex];

            var leftCount = count / 2;
            var leftAggregate = Recurse(source, startIndex, leftCount, func);

            var rightCount = count - leftCount;
            var rightAggregate = Recurse(source, startIndex + leftCount, rightCount, func);

            return func(leftAggregate, rightAggregate);
        }
    }
}
