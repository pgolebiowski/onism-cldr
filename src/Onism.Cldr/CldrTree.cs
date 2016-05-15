using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;
using ProtoBuf;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents CLDR data (of standard type) in a hierarchical way.
    /// </summary>
    [ProtoContract]
    public class CldrTree
    {
        [ProtoMember(1)]
        internal readonly CldrTreeNode Root;

        [ProtoMember(2)]
        internal readonly List<string> Values;

        [ProtoMember(3)]
        internal readonly Dictionary<CldrLocale, int> Locales;

        internal CldrTree()
        {
            this.Root = new CldrTreeNode(this, null);
            this.Values = new List<string>();
            this.Locales = new Dictionary<CldrLocale, int>();
        }

        public CldrTreeNode SelectNode(string path) => this.Root.SelectNode(path);

        internal void Add(CldrJson cldrJson)
        {
            if (cldrJson == null)
                return;

            foreach (var leaf in cldrJson.Data.Leaves())
            {
                Add(cldrJson.Locale, leaf.Path, (string)leaf);
            }
        }

        private void Add(CldrLocale locale, string path, string value)
        {
            var treePath = CldrTreePath.Parse(path);
            var localeId = Locales.GetOrAddId(locale ?? CldrLocale.None);
            var valueId = Values.GetOrAddId(value);

            Root.Add(localeId, treePath, valueId);
        }

        [ProtoAfterDeserialization]
        protected void OnDeserialized()
        {
            SetPropertiesRecursively(this, Root);
        }

        private static void SetPropertiesRecursively(CldrTree tree, CldrTreeNode node)
        {
            foreach (var child in node.PropertyChildren.Values)
            {
                child.Tree = tree;
                child.Parent = node;
                SetPropertiesRecursively(tree, child);
            }
            foreach (var child in node.ArrayChildren)
            {
                child.Tree = tree;
                child.Parent = node;
                SetPropertiesRecursively(tree, child);
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
    }
}