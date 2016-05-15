using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.Subsetting;

namespace Onism.Cldr.Test.Utils
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Removes tokens from this JSON token using a collection of patterns.
        /// This naive algorithm marks all the affected leaves every time
        /// a pattern is evaluated.
        /// </summary>
        public static void NaiveRemove(this JToken root, PatternCollection patterns)
        {
            var remover = new NaiveTokenRemover();
            remover.Remove(root, patterns);
        }
    }
}