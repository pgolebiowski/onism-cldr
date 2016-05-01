using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Extensions
{
    public static class JTokenExtensions
    {
        public static string ToPrettyString(this JToken token)
        {
            return token.ToString(Formatting.Indented);
        }
    }
}
