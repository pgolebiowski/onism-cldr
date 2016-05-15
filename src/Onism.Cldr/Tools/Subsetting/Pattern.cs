using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Tools.Subsetting
{
    /// <summary>
    /// Represents a pattern based on a JPath expression.
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// Gets the JPath expression of this pattern.
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Gets a flag indicating whether the pattern is negated.
        /// Any matching path excluded by a previous pattern will become included again.
        /// </summary>
        public bool IsNegated { get; }

        private Pattern(string expression, bool isNegated)
        {
            this.Expression = expression;
            this.IsNegated = isNegated;
        }

        /// <summary>
        /// Loads a <see cref="Pattern"/> from a string.
        /// </summary>
        public static Pattern Parse(string path)
        {
            Validate(path);

            var isNegated = false;
            if (path.StartsWith("!"))
            {
                path = path.TrimStart('!');
                isNegated = true;
            }

            return new Pattern(path, isNegated);
        }

        public static void Validate(string path)
        {
            // this 'hack' is in fact a really good way to validate a JSON path.
            // before executing the method 'SelectTokens', Json.NET library
            // uses internal validation against a Pattern expression. That's exactly
            // what is needed to do here.
            JObject.Parse("{}").SelectTokens(path);
        }

        public override string ToString()
        {
            var exclamation = this.IsNegated ? "!" : "";
            return exclamation + this.Expression;
        }
    }
}