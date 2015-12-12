using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;
using Onism.Cldr.Packages;
using ProtoBuf;

namespace Onism.Cldr
{
    // Some facts:
    // - the maximum depth of CLDR data is ~8
    // - there are ~19,300 leaves (out of ~22,800 vertices in total)
    // - there are ~6,262,000 key-value pairs in leaves (and they take about 85% of memory)
    // - counting separately for each leaf, there are only 654,000 distinct values in dictionaries
    // - about 50% of all the dictionaries in leaves contain data for all the locales

    /// <summary>
    /// Represents CLDR data in a hierarchical way.
    /// </summary>
    [ProtoContract]
    public class CldrTree
    {
        [ProtoMember(1)]
        internal readonly CldrTreeNode Root;

        [ProtoMember(2)]
        internal readonly Dictionary<string, int> Values;

        [ProtoMember(3)]
        internal readonly Dictionary<CldrLocale, int> Locales;

        internal CldrTree()
        {
            Root = new CldrTreeNode(this, null);
            Values = new Dictionary<string, int>();
            Locales = new Dictionary<CldrLocale, int>();
        }

        internal string GetValueById(int id)
        {
            return Values.First(x => x.Value == id).Key;
        }

        public CldrTreeNode SelectNode(string path) => Root.SelectNode(path);

        internal void Add(CldrJson cldrJson)
        {
            foreach (var leaf in cldrJson.Data.Leaves())
            {
                Add(cldrJson.Locale, leaf.Path, (string) leaf);
            }
        }

        private void Add(CldrLocale locale, string path, string value)
        {
            var pathData = path.Split('.');

            var localeId = Locales.GetOrAddId(locale);
            var valueId = Values.GetOrAddId(value);

            Root.Add(localeId, pathData, valueId);
        }

        [ProtoAfterDeserialization]
        protected void OnDeserialized()
        {
            SetParentRecursive(Root);
        }

        private static void SetParentRecursive(CldrTreeNode node)
        {
            foreach (var child in node.Children.Values)
            {
                child.Parent = node;
                SetParentRecursive(child);
            }
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode() ^ Locales.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrTree;

            if (other == null)
                return false;

            var valuesEqual = Values.IsSameAs(other.Values);
            var localesEqual = Locales.IsSameAs(other.Locales);
            var childrenEqual = Root.Equals(other.Root);

            return valuesEqual && localesEqual && childrenEqual;
        }

        /// <summary>
        /// Convert this tree into a more compact format (allowing to specify a subset of the data).
        /// </summary>
        public void Optimize()
        {
            // TODO: add CldrTreeSchema 
        }
    }
}
