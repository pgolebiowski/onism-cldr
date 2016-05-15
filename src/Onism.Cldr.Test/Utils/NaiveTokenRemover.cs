using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Tools.Subsetting;

namespace Onism.Cldr.Test.Utils
{
    /// <summary>
    /// Represents an object capable of removing descendant tokens
    /// of <see cref="JToken"/> in a similar way to how gitignore handles
    /// patterns. This naive implementation is used to test another algorithm.
    /// </summary>
    public class NaiveTokenRemover
    {
        private readonly ObjectIDGenerator objectIdGenerator; // crucial

        public NaiveTokenRemover()
        {
            this.objectIdGenerator = new ObjectIDGenerator();
        }

        /// <summary>
        /// Removes tokens from this JSON token using a collection of patterns.
        /// This naive algorithm marks all the affected leaves every time
        /// a pattern is evaluated.
        /// </summary>
        public void Remove(JToken root, PatternCollection patterns)
        {
            var leaves = ((JContainer) root).DescendantsAndSelf().OfType<JValue>().ToArray();
            var exclusionDictionary = leaves.ToDictionary(GetId, x => false);

            foreach (var pattern in patterns)
                foreach (var matched in root.SelectTokens(pattern.Expression))
                    foreach (var leaf in FindLeaves(matched))
                        exclusionDictionary[GetId(leaf)] = !pattern.IsNegated;

            var toRemove = leaves.Where(x => exclusionDictionary[GetId(x)]);
            var queue = new Queue<JToken>(toRemove);

            while (queue.IsNotEmpty())
            {
                var toHandle = queue.Dequeue();
                if (!toHandle.HasValues)
                {
                    var parent = toHandle.RemoveFromParent();
                    if (parent != null)
                        queue.Enqueue(parent);
                }
            }
        }

        private static IEnumerable<JToken> FindLeaves(JToken token)
        {
            if (!token.HasValues)
            {
                yield return token;
                yield break;
            }

            foreach (var child in token.Children())
                foreach (var found in FindLeaves(child))
                    yield return found;
        }

        private long GetId(JToken token)
        {
            bool firstTime;
            return this.objectIdGenerator.GetId(token, out firstTime);
        }
    }
}