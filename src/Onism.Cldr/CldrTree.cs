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
        internal readonly IList<string> Values;

        [ProtoMember(3)]
        internal readonly IdentifierDictionary<CldrLocale> Locales;

        internal CldrTree()
        {
            this.Root = new CldrTreeNode(this, null);
            this.Values = new List<string>();
            this.Locales = new IdentifierDictionary<CldrLocale>();
        }

        public CldrTreeNode SelectNode(string path) => this.Root.SelectNode(path);

        public void Add(CldrLocale locale, string path, int valueId)
        {
            var treePath = CldrTreePath.Parse(path);
            var localeId = this.Locales.GetId(locale ?? CldrLocale.None);

            this.Root.Add(localeId, treePath, valueId);
        }

        [ProtoAfterDeserialization]
        protected void OnDeserialized()
        {
            SetPropertiesRecursively(this, this.Root);
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
            return this.Values.GetHashCode() ^ this.Locales.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrTree;

            if (other == null)
                return false;

            var valuesEqual = this.Values.IsSameAs(other.Values);
            var localesEqual = this.Locales.Equals(other.Locales);
            var childrenEqual = this.Root.Equals(other.Root);

            return valuesEqual && localesEqual && childrenEqual;
        }
    }
}