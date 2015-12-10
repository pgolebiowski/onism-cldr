using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Extensions
{
    public static class JTokenExtensions
    {
        public static HashSet<JTokenType> GetAllTypes(this JToken token)
        {
            var types = new HashSet<JTokenType> {token.Type};

            foreach (var child in token.Children())
                foreach (var type in child.GetAllTypes())
                    types.Add(type);

            return types;
        }

        public static IEnumerable<JToken> FindLeaves(this JToken token)
        {
            var leaves = new List<JToken>();

            if (token is JValue)
                leaves.Add(token);

            leaves.AddRange(token.SelectMany(x => x.FindLeaves()));
            return leaves;
        }
    }
}
