using System.Collections.Generic;

namespace Onism.Cldr.Extensions
{
    public static class DictionaryExtensions
    {
        public static int GetOrAddId<TKey>(this IDictionary<TKey, int> dict, TKey key)
        {
            int id;

            if (dict.TryGetValue(key, out id))
                return id;

            id = dict.Count;
            dict.Add(key, id);
            return id;
        }
    }
}