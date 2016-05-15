using System.Collections.Generic;

namespace Onism.Cldr.Tools.Subsetting
{
    public class PatternCollectionBuilder
    {
        private readonly List<Pattern> patterns;

        public PatternCollectionBuilder()
        {
            this.patterns = new List<Pattern>();
        }

        public PatternCollectionBuilder Exclude(string path)
        {
            var pattern = Pattern.Parse(path);
            this.patterns.Add(pattern);
            return this;
        }

        public PatternCollectionBuilder Include(string path)
        {
            var pattern = Pattern.Parse($"!{path}");
            this.patterns.Add(pattern);
            return this;
        }

        public PatternCollection Build()
        {
            return new PatternCollection(this.patterns);
        }
    }
}