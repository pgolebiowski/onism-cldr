using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.Subsetting;

namespace Onism.Cldr.Test.Utils
{
    /// <summary>
    /// Represents a generator of <see cref="PatternCollection"/> instances.
    /// Uses <see cref="DeterministicRandom"/> to generate randomized
    /// yet deterministic output.
    /// </summary>
    public class PatternCollectionGenerator
    {
        private readonly DeterministicRandom random;

        /// <summary>
        /// Initializes a new instance of <see cref="PatternCollectionGenerator"/>,
        /// using <see cref="DeterministicRandom"/> to get deterministic output.
        /// </summary>
        public PatternCollectionGenerator(DeterministicRandom random)
        {
            this.random = random;
        }

        /// <summary>
        /// Generates a collection of patterns for the specified <see cref="JContainer"/>.
        /// </summary>
        public PatternCollection GeneratePatterns(JContainer root, int desiredCount, bool skipValues = true)
        {
            var descendants = root.DescendantsAndSelf().ToArray();
            descendants.Shuffle(random);

            if (skipValues)
                descendants = descendants.Where(x => x is JObject || x is JArray).ToArray();

            var patterns = descendants
                .Select(x => x.Path)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(desiredCount)
                .Select(x => (random.NextBool() ? "!" : "") + x)
                .Select(AddWildcards)
                .ToArray();

            return PatternCollection.Parse(patterns);
        }
        
        /// <remarks>
        /// This method relies on a feature of <see cref="JContainerGenerator"/> that
        /// the keys of properties are only non-negative numbers, chosen
        /// sequentially from 0 and not exceeding 9.
        /// </remarks>
        private string AddWildcards(string path)
        {
            var indexesToChange = new List<int>();

            for (var i = 0; i < path.Length; ++i)
            {
                var c = path[i];
                if (char.IsDigit(c))
                {
                    var useWildcard = random.NextInt() % 5 == 0; // 20% chance
                    if (useWildcard)
                        indexesToChange.Add(i);
                }
            }

            var builder = new StringBuilder(path);
            foreach (var index in indexesToChange)
                builder[index] = '*';

            return builder.ToString();
        }
    }
}
