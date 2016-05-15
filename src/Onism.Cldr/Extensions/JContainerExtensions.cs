using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Extensions
{
    internal static class JContainerExtensions
    {
        /// <summary>
        /// Returns a collection of the tokens that are leaves in this tree.
        /// </summary>
        public static IEnumerable<JValue> Leaves(this JContainer obj)
        {
            return obj
                .DescendantsAndSelf()
                .Where(x => x is JValue)
                .Cast<JValue>();
        }
    }
}
