using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Tools.Subsetting
{
    /// <summary>
    /// Represents a dictionary of JSON tokens and decisions to be made
    /// when they're encouneted during post-order traversal.
    /// </summary>
    public class DecisionDictionary : Dictionary<long, Decision>
    {
        private int time;
        private readonly ObjectIDGenerator objectIdGenerator; // crucial

        public DecisionDictionary() : this(new Dictionary<long, Decision>())
        {
        }

        private DecisionDictionary(IDictionary<long, Decision> dictionary) : base(dictionary)
        {
            this.time = 0;
            this.objectIdGenerator = new ObjectIDGenerator();
        }

        /// <summary>
        /// Adds or updates decisions for the specified tokens.
        /// </summary>
        public void AddOrUpdateFor(IEnumerable<JToken> tokens, bool toExclude)
        {
            foreach (var token in tokens)
                this[GetId(token)] = new Decision(this.time, toExclude);
            
            ++this.time;
        }

        /// <summary>
        /// Gets the decision for how to handle the token.
        /// </summary>
        public Decision GetFor(JToken token)
        {
            Decision result;
            this.TryGetValue(GetId(token), out result);
            return result;
        }

        private long GetId(JToken token)
        {
            bool firstTime;
            var id = this.objectIdGenerator.GetId(token, out firstTime);
            return id;
        }
    }
}