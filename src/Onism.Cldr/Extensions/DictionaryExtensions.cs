using System.Collections.Generic;

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
    }
}