using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Packages;
using Onism.Cldr.Test.Extensions;

namespace Onism.Cldr.Test.Utils
{
    internal static class CldrJsonGenerator
    {
        private static string Str => Guid.NewGuid().ToString();

        public static CldrJson Generate(int arity, int depth)
        {
            return new CldrJson(null, CldrLocale.None, CreateNode(arity, depth));
        }

        /// <summary>
        /// Returns an array of CldrJson objects, where their data has trees similar in "similarity" %.
        /// Similarity is the % of leaves (and thus paths to them) that are common between all trees.
        /// </summary>
        public static CldrJson[] Generate(IEnumerable<CldrLocale> locales, int arity, int depth, double similarity, double uniqueLeavesAmongCommon, bool refill = false)
        {
            // in order to create a collection of trees with a certain density of their subtrees,
            // simply create one full tree and then remove a proper amount of vertices
            // for each locale's duplicate. The removal is of course done recursively and entirely,
            // so no nodes are left without values somewhere deep down.

            var tree = CreateNode(arity, depth);
            // remove leaves
            var leaves = tree.Leaves().ToArray();
            var toRemove = leaves.Length * (1 - similarity);
            leaves.Shuffle();
            leaves.Take((int)toRemove).ForEach(x => x.Parent.Remove());

            // remove redundant vertices
            RemoveWithoutDescendantValues(tree);

            var trees = locales
                .Select(x => new {Locale = x, Data = (JObject) tree.DeepClone()})
                .ToArray();

            foreach (var t in trees)
            {
                // go to leaves and set new values to half of them
                leaves = t.Data.Leaves().ToArray();
                leaves.Shuffle();
                var toModify = leaves.Length * (1 - uniqueLeavesAmongCommon);
                leaves.Take((int)toModify).ForEach(x => ((JValue)x).Value = Str);

                // refill
                if (refill)
                    Refill(t.Data, arity, depth);
            }

            return trees
                .Select(x => new CldrJson(null, x.Locale, x.Data))
                .ToArray();
        }

        /// <summary>
        /// Removes all nodes without any descendant values.
        /// </summary>
        private static void RemoveWithoutDescendantValues(JObject obj)
        {
            foreach (var property in obj.Properties().ToArray())
            {
                if (property.Value.Type != JTokenType.String)
                    RemoveWithoutDescendantValues((JObject) property.Value);
            }

            if (obj.HasValues == false)
                obj.Parent.Remove();
        }

        /// <summary>
        /// Makes the tree full once again.
        /// </summary>
        public static void Refill(JObject obj, int arity, int depth)
        {
            var childrenCount = obj.Children().Count();
            var instances = Enumerable.Range(1, arity - childrenCount);

            if (depth == 1)
                instances.ForEach(x => obj.Add(Str, Str));
            else
            {
                foreach (var property in obj.Properties())
                {
                    Refill((JObject) property.Value, arity, depth - 1);
                }

                instances.ForEach(x => obj.Add(Str, CreateNode(arity, depth - 1)));
            }
        }

        /// <summary>
        /// Creates a full tree with the specified properties.
        /// </summary>
        private static JObject CreateNode(int arity, int depth)
        {
            var instances = Enumerable.Range(1, arity);
            var o = new JObject();

            if (depth == 1)
                instances.ForEach(x => o.Add(Str, Str));
            else
                instances.ForEach(x => o.Add(Str, CreateNode(arity, depth - 1)));

            return o;
        }
    }
}