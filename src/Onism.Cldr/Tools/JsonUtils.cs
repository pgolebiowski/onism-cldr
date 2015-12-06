using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Tools
{
    internal static class JsonUtils
    {
        /// <summary>
        /// Returns a <see cref="JObject"/> loaded from a JSON file.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        public static JObject LoadFromFile(string path)
        {
            var allText = File.ReadAllText(path);
            return JObject.Parse(allText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array">The string representation of a char array.</param>
        public static char[] ParseCharArray(string array)
        {
            return array
                .Trim('[', ']')
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray();
        }
    }
}
