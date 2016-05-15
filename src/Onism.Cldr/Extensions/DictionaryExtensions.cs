using System.Collections.Generic;
using System.Linq;

namespace Onism.Cldr.Extensions
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Gets a flag indicating whether two dictionaries have exactly same contents
        /// by using the default equality comparer to compare values.
        /// </summary>
        public static bool IsSameAs<TKey, TValue>(this Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            return dict1.Count == dict2.Count && dict1.Except(dict2).IsEmpty();
        }
    }
}