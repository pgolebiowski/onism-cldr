using System;
using System.Collections.Generic;
using System.Linq;
using Onism.Cldr.Extensions;
using ProtoBuf;

namespace Onism.Cldr
{
    [ProtoContract]
    public class CldrTreeNode
    {
        /// <remarks>
        /// This property is set automatically right after deserialization.
        /// IN ORDER TO PRESERVE INDEXES, NODES MUST BE ADDED IN DOCUMENT ORDER!!!
        /// </remarks>
        internal CldrTree Tree { get; set; }

        /// <remarks>
        /// This property is set automatically right after deserialization.
        /// </remarks>
        internal CldrTreeNode Parent { get; set; }

        [ProtoMember(1)]
        public readonly Dictionary<string, CldrTreeNode> PropertyChildren = new Dictionary<string, CldrTreeNode>();

        [ProtoMember(2)]
        public readonly List<CldrTreeNode> ArrayChildren = new List<CldrTreeNode>();

        [ProtoMember(3)]
        internal readonly Dictionary<int, int> LocaleValues = new Dictionary<int, int>();

        public CldrTreeNode()
        {

        }

        internal CldrTreeNode(CldrTree tree, CldrTreeNode parent)
        {
            this.Tree = tree;
            this.Parent = parent;
        }

        public string GetValue(CldrLocale locale)
        {
            var localeId = this.Tree.Locales.GetId(locale);
            var valueId = this.LocaleValues[localeId];
            var value = this.Tree.Values[valueId];
            return value;
        }

        public override int GetHashCode()
        {
            return this.PropertyChildren.GetHashCode()
                ^ this.ArrayChildren.GetHashCode()
                ^ this.LocaleValues.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrTreeNode;

            if (other == null)
                return false;

            var keyedChildrenEqual = this.PropertyChildren.IsSameAs(other.PropertyChildren);
            var indexedChildrenEqual = this.ArrayChildren.IsEquivalentTo(other.ArrayChildren);
            var localeValuesEqual = this.LocaleValues.IsSameAs(other.LocaleValues);

            return keyedChildrenEqual && indexedChildrenEqual && localeValuesEqual;
        }

        /// <summary>
        /// Returns a collection of the ancestor nodes of this node.
        /// </summary>
        /// <returns>A collection of the ancestor nodes of this node.</returns>
        public IEnumerable<CldrTreeNode> Ancestors()
        {
            return GetAncestors(false);
        }

        /// <summary>
        /// Returns a collection of tokens that contain this token, and the ancestors of this token.
        /// </summary>
        /// <returns>A collection of tokens that contain this token, and the ancestors of this token.</returns>
        public IEnumerable<CldrTreeNode> AncestorsAndSelf()
        {
            return GetAncestors(true);
        }

        private IEnumerable<CldrTreeNode> GetAncestors(bool self)
        {
            for (var current = self ? this : this.Parent; current != null; current = current.Parent)
            {
                yield return current;
            }
        }

        /// <summary>
        /// Gets the name this node's parent uses to label it.
        /// </summary>
        public string Key
        {
            get
            {
                return this.Parent
                    ?.PropertyChildren
                    ?.FirstOrDefault(x => ReferenceEquals(x.Value, this))
                    .Key;
            }
        }

        public int Index
        {
            get
            {
                var parentArray = this.Parent?.ArrayChildren;
                
                if (parentArray == null)
                    throw new NullReferenceException();

                for (var i = 0; i < parentArray.Count; ++i)
                    if (ReferenceEquals(parentArray[i], this))
                        return i;

                throw new NullReferenceException();
            }
        }

        /// <summary>
        /// Selects a tree node using a dot-separated expression.
        /// </summary>
        /// <param name="path">The dot-separated expression.</param>
        public CldrTreeNode SelectNode(string path)
        {
            return SelectNode(CldrTreePath.Parse(path));
        }

        private CldrTreeNode SelectNode(CldrTreePath path)
        {
            // that's the node we've been looking for!
            if (path.IsEmpty())
                return this;

            // we need to go deeper
            CldrTreeNode child;
            var pathSegment = path.Dequeue();

            if (pathSegment.IsDictionaryKey)
            {
                if (this.PropertyChildren.TryGetValue(pathSegment.Key, out child) == false)
                    return null;
            }
            else
            {
                if (this.ArrayChildren.TryGetElement(pathSegment.Index, out child) == false)
                    return null;
            }

            return child.SelectNode(path);
        }

        internal void Add(int locale, CldrTreePath path, int value)
        {
            // that's the node we've been looking for!
            if (path.IsEmpty())
            {
                this.LocaleValues.Add(locale, value);
                return;
            }

            // we need to go deeper
            CldrTreeNode child;
            var pathSegment = path.Dequeue();

            if (pathSegment.IsDictionaryKey)
            {
                if (this.PropertyChildren.TryGetValue(pathSegment.Key, out child) == false)
                {
                    child = new CldrTreeNode(this.Tree, this);
                    this.PropertyChildren.Add(pathSegment.Key, child);
                }
            }
            else
            {
                if (this.ArrayChildren.TryGetElement(pathSegment.Index, out child) == false)
                {
                    child = new CldrTreeNode(this.Tree, this);
                    this.ArrayChildren.Add(child);
                }
            }

            child.Add(locale, path, value);
        }
    }
}