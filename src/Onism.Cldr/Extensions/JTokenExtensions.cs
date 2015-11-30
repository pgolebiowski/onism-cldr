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
            var types = new HashSet<JTokenType>();
            types.Add(token.Type);

            foreach (var child in token.Children())
                foreach (var type in child.GetAllTypes())
                    types.Add(type);

            return types;
        }

        public static IEnumerable<JToken> FindLeaves(this JToken token)
        {
            var children = token.Children();

            if (children.IsNotEmpty())
                return children.SelectMany(x => x.FindLeaves());
            else
                return token;
        }
    }
}
