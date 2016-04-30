using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Subsetting
{
    /// <summary>
    /// Represents a dictionary of JSON tokens and decisions to be made
    /// when they're encouneted during post-order traversal.
    /// </summary>
    public class DecisionDictionary : Dictionary<JToken, Decision>
    {
        private int time = 0;

        public DecisionDictionary() : this(new Dictionary<JToken, Decision>())
        {
        }

        private DecisionDictionary(IDictionary<JToken, Decision> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// Adds or updates decisions for the specified tokens.
        /// </summary>
        public void AddOrUpdateFor(IEnumerable<JToken> tokens, bool toExclude)
        {
            foreach (var token in tokens)
                this[token] = new Decision(time, toExclude);

            ++time;
        }

        /// <summary>
        /// Gets the decision for how to handle the token.
        /// </summary>
        public Decision GetFor(JToken token)
        {
            Decision result;
            this.TryGetValue(token, out result);
            return result;
        }
    }
}