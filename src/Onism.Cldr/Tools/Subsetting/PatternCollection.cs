using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Tools.Subsetting
{
    /// <summary>
    /// Represents a read-only collection of patterns.
    /// </summary>
    public class PatternCollection : ReadOnlyCollection<Pattern>
    {
        internal PatternCollection(IList<Pattern> list) : base(list)
        {
        }

        /// <summary>
        /// Loads a <see cref="PatternCollection"/> from a string
        /// that contains a list of patterns.
        /// </summary>
        public static PatternCollection Parse(string patterns)
        {
            var linesWithPatterns = patterns
                .EnumerateLines()
                .Where(line => !string.IsNullOrWhiteSpace(line)) // empty lines
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("#")); // comments

            return Parse(linesWithPatterns);
        }

        /// <summary>
        /// Loads a <see cref="PatternCollection"/> from a collection
        /// of patterns represented as strings.
        /// </summary>
        public static PatternCollection Parse(IEnumerable<string> patterns)
        {
            var collection = patterns
                .Select(Pattern.Parse)
                .ToArray();

            return new PatternCollection(collection);
        }

        /// <summary>
        /// Gets a new empty <see cref="PatternCollection"/>. It will match
        /// nothing and thus nothing will be removed during subsetting.
        /// </summary>
        public static PatternCollection Empty => new PatternCollection(new List<Pattern>());
    }
}
