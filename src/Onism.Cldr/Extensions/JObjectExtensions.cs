using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Extensions
{
    internal static class JObjectExtensions
    {
        /// <summary>
        /// Returns a collection of all the types found in this token and its child tokens.
        /// </summary>
        public static IEnumerable<JTokenType> GetAllTypes(this JObject obj)
        {
            return obj
                .DescendantsAndSelf()
                .Select(x => x.Type)
                .Distinct();
        }

        /// <summary>
        /// Returns a collection of the tokens that are leaves in this subtree.
        /// </summary>
        public static IEnumerable<JToken> Leaves(this JObject obj)
        {
            return obj
                .DescendantsAndSelf()
                .Where(x => x is JValue);
        }

        /// <summary>
        /// Returns a collection of the paths to tokens that are leaves in this subtree.
        /// </summary>
        public static IEnumerable<string> LeafPaths(this JObject obj)
        {
            return obj
                .Leaves()
                .Select(x => x.Path);
        }

        /// <summary>
        /// Returns a dictionary of the paths to the tokens that are leaves in this subtree,
        /// mapped with their values.
        /// </summary>
        public static Dictionary<string, string> LeafPathsAndValues(this JObject obj)
        {
            return obj
                .Leaves()
                .ToDictionary(x => x.Path, x => (string) x);
        }

        public static IEnumerable<T> Descendants<T>(this JObject obj)
        {
            return obj.DescendantsAndSelf()
                .Where(x => x is T)
                .Cast<T>();
        }

        public static IEnumerable<T> Descendants<T>(this JObject obj, Func<T, bool> predicate)
        {
            return obj.Descendants<T>()
                .Where(predicate);
        } 

        public static IEnumerable<JValue> ValuesByType(this JObject obj, JTokenType valueType)
        {
            return obj.Descendants<JValue>()
                .Where(x => x.Type == valueType);
        } 
    }
}
