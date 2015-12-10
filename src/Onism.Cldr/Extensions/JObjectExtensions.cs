using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Extensions
{
    public static class JObjectExtensions
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
    }
}
