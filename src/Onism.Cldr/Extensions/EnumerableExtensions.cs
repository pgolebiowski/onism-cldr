using System.Collections.Generic;
using System.Linq;

namespace Onism.Cldr.Extensions
{
    public static class EnumerableExtensions
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
        /// Concatenates the members of a collection, using the specified separator between each member.
        /// </summary>
        public static string JoinStrings<T>(this IEnumerable<T> source, string separator = ", ")
        {
            return string.Join(separator, source);
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
    }
}
