using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Tools.Subsetting
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Removes tokens from this JSON token using a collection of patterns.
        /// </summary>
        public static void Subset(this JToken root, PatternCollection patterns)
        {
            var decisions = new DecisionDictionary();

            foreach (var pattern in patterns)
            {
                var matchedTokens = root.SelectTokens(pattern.Expression);
                var toExclude = !pattern.IsNegated;
                decisions.AddOrUpdateFor(matchedTokens, toExclude);
            }
            
            Subset(root, decisions.GetFor(root), decisions);
        }

        /// <summary>
        /// Traverses the JSON in post-order, removing tokens bottom-up.
        /// </summary>
        private static void Subset(JToken token, Decision decision, DecisionDictionary decisions)
        {
            foreach (var child in token.Children().ToArray())
            {
                var inherited = decision;
                var decisionForChild = Decision.GetNewer(inherited, decisions.GetFor(child));

                Subset(child, decisionForChild, decisions);
            }

            var isLeafToExclude = token is JValue && decision != null && decision.Remove;
            var isChildlessAncestor = !(token is JValue) && !token.HasValues;

            if (isLeafToExclude || isChildlessAncestor)
                token.RemoveFromParent();
        }

        /// <summary>
        /// Removes this token from its parent. Assumes only JArray, JObject,
        /// JProperty, or JValue are to be encountered.
        /// </summary>
        public static JToken RemoveFromParent(this JToken token)
        {
            var parent = token.Parent;
            if (parent == null)
                return null;

            if (parent is JArray || parent is JObject)
            {
                token.Remove();
                return parent;
            }

            if (parent is JProperty)
                return RemoveFromParent(parent);

            throw new Exception($"The parent is {token.Parent.Type}, which was not expected.");
        }
    }
}
