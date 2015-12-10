using System.Collections.Generic;

namespace Onism.Cldr.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the id associated with the specified key if it exists. If not,
        /// adds the next id and returns its value.
        /// </summary>
        /// <remarks>
        /// This method assumes values will not be removed from a dictionary (potential
        /// collision of identifiers). If a dictionary can have its values removed and then
        /// added, use a different way to obtain the next id.
        /// </remarks>
        public static int GetOrAddId<TKey>(this IDictionary<TKey, int> dict, TKey key)
        {
            int id;

            if (dict.TryGetValue(key, out id))
                return id;

            // dangerous if removing elements!
            id = dict.Count;

            // slower, but safe solution
            // id = dict.IsEmpty() ? 0 : dict.Values.Max() + 1;

            dict.Add(key, id);
            return id;
        }
    }
}