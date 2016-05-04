using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onism.Cldr.Extensions
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Gets a flag indicating whether the collection is empty.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        /// <summary>
        /// Gets a flag indicating whether the collection contains any elements.
        /// </summary>
        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            return source.Any();
        }

        /// <summary>
        /// Concatenates members of the collection using the specified separator between each member.
        /// </summary>
        public static string JoinStrings<T>(this IEnumerable<T> source, string separator = ", ")
        {
            return string.Join(separator, source);
        }

        /// <summary>
        /// Concatenates members of the collection so that each is in separate line.
        /// </summary>
        public static string MergeLines<T>(this IEnumerable<T> source)
        {
            var builder = new StringBuilder();
            foreach (var s in source)
                builder.AppendLine(s.ToString());

            return builder.ToString();
        }

        /// <summary>
        /// Wraps this object instance into an <see cref="IEnumerable{T}"/>
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// The <see cref="Array"/> and <see cref="List{T}"/> classes already have ForEach methods,
        /// though it is then necessary to actually create those new objects so that one can iterate
        /// over them. That is bad. So bad. This is better ;)
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static IEnumerable<int> GetIndexes<T>(this T[] source, Func<T, bool> predicate)
        {
            for (var i = 0; i < source.Length; ++i)
                if (predicate(source[i]))
                    yield return i;
        }

        /// <summary>
        /// Determines whether this array is a prefix of another - that all its elements
        /// at corresponding indexes in the other array are equal.
        /// </summary>
        public static bool IsPrefixOf<T>(this IReadOnlyList<T> source, IReadOnlyList<T> other)
        {
            var toCheck = source.Count;

            if (toCheck > other.Count)
                return false;

            for (var i = 0; i < toCheck; ++i)
            {
                if (object.Equals(source[i], other[i]) == false)
                    return false;
            }

            return true;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}
