using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Tools
{
    public class JsonMerger
    {
        public static JObject SafeMerge(JObject o1, JObject o2)
        {
            var leaves1 = o1.FindLeaves().ToDictionary(x => x.Path, x => (string) x);
            var leaves2 = o2.FindLeaves().ToDictionary(x => x.Path, x => (string) x);

            o1.Merge(o2);

            var leavesMerged = o1.FindLeaves().ToDictionary(x => x.Path, x => (string) x);

            // now assert
            var allExist = leaves1.All(x => leavesMerged[x.Key] == x.Value)
                           && leaves2.All(x => leavesMerged[x.Key] == x.Value);

            if (allExist == false)
                throw new Exception();

            return o1;
        }
    }
}