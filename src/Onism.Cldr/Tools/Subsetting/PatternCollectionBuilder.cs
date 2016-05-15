using System.Collections.Generic;

namespace Onism.Cldr.Tools.Subsetting
{
    /// <summary>
    /// Represents an object that facilitates building
    /// a <see cref="PatternCollection"/> from the code.
    /// </summary>
    public class PatternCollectionBuilder
    {
        private readonly List<Pattern> patterns;

        public PatternCollectionBuilder()
        {
            this.patterns = new List<Pattern>();
        }

        /// <summary>
        /// Parses the path to create an excluding pattern
        /// and adds it to the end of the collection.
        /// </summary>
        public PatternCollectionBuilder Exclude(string path)
        {
            var pattern = Pattern.Parse(path);
            this.patterns.Add(pattern);
            return this;
        }

        /// <summary>
        /// Parses the path to create an including pattern
        /// and adds it to the end of the collection.
        /// </summary>
        public PatternCollectionBuilder Include(string path)
        {
            var pattern = Pattern.Parse($"!{path}");
            this.patterns.Add(pattern);
            return this;
        }

        /// <summary>
        /// Builds a collection of patterns.
        /// </summary>
        public PatternCollection Build()
        {
            return new PatternCollection(this.patterns);
        }
    }
}