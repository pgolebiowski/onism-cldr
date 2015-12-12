using System.Collections.Generic;
using System.Linq;

namespace Onism.Cldr.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the id associated with the specified key. If the key is missing,
        /// it is added and its new id returned.
        /// </summary>
        public static int GetOrAddId<TKey>(this Dictionary<TKey, int> dict, TKey key)
        {
            int id;

            if (dict.TryGetValue(key, out id))
                return id;

            // no removal will be done, so it's safe
            id = dict.Count;

            dict.Add(key, id);
            return id;
        }

        public static bool IsSameAs<TKey, TValue>(this Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            return dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
        }
    }
}