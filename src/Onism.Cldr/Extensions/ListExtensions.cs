using System.Collections.Generic;
using System.Linq;

namespace Onism.Cldr.Extensions
{
    internal static class ListExtensions
    {
        /// <summary>
        /// Gets a flag indicating whether two lists have exactly same contents
        /// by using the default equality comparer to compare elements.
        /// The order does not matter.
        /// </summary>
        public static bool IsEquivalentTo<T>(this IList<T> list1, IList<T> list2)
        {
            return list1.Count == list2.Count && list1.Except(list2).IsEmpty();
        }

        /// <summary>
        /// Tries to get the element at the specified index. On fail, false is returned.
        /// </summary>
        public static bool TryGetElement<T>(this IList<T> list, int index, out T element)
        {
            if (index >= 0 && index < list.Count)
            {
                element = list[index];
                return true;
            }
            else
            {
                element = default(T);
                return false;
            }
        }
    }
}