using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;

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

        public static JObject SafeMerge(JObject o1, JObject o2)
        {
            var leaves1 = o1.FindLeaves().ToDictionary(x => x.Path, x => (string)x);
            var leaves2 = o2.FindLeaves().ToDictionary(x => x.Path, x => (string)x);

            o1.Merge(o2);

            var leavesMerged = o1.FindLeaves().ToDictionary(x => x.Path, x => (string)x);

            // now assert
            var allExist = leaves1.All(x => leavesMerged[x.Key] == x.Value)
                           && leaves2.All(x => leavesMerged[x.Key] == x.Value);

            if (allExist == false)
                throw new Exception();

            return o1;
        }
    }
}
