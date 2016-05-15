using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Tree = tree;
            Parent = parent;
        }

        public string GetValue(CldrLocale locale)
        {
            var localeId = Tree.Locales[locale];
            var valueId = LocaleValues[localeId];
            var value = Tree.Values[valueId];
            return value;
        }

        public override int GetHashCode()
        {
            return PropertyChildren.GetHashCode()
                ^ ArrayChildren.GetHashCode()
                ^ LocaleValues.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrTreeNode;

            if (other == null)
                return false;

            var keyedChildrenEqual = PropertyChildren.IsSameAs(other.PropertyChildren);
            var indexedChildrenEqual = ArrayChildren.IsSameAs(other.ArrayChildren);
            var localeValuesEqual = LocaleValues.IsSameAs(other.LocaleValues);

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
            for (var current = self ? this : Parent; current != null; current = current.Parent)
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
                return Parent
                    ?.PropertyChildren
                    ?.FirstOrDefault(x => ReferenceEquals(x.Value, this))
                    .Key;
            }
        }

        public int Index
        {
            get
            {
                var parentArray = Parent?.ArrayChildren;
                
                if (parentArray == null)
                    throw new NullReferenceException();

                for (var i = 0; i < parentArray.Count; ++i)
                    if (ReferenceEquals(parentArray[i], this))
                        return i;

                throw new NullReferenceException();
            }
        }

        /// <summary>
        /// Gets the path of this node. 
        /// </summary>
        public string Path
        {
            get
            {
                throw new NotImplementedException();

                return AncestorsAndSelf()
                    .Select(x => x.Key)
                    .Reverse()
                    .JoinStrings(".");
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
                if (PropertyChildren.TryGetValue(pathSegment.Key, out child) == false)
                    return null;
            }
            else
            {
                if (ArrayChildren.TryGetElement(pathSegment.Index, out child) == false)
                    return null;
            }

            return child.SelectNode(path);
        }

        internal void Add(int locale, CldrTreePath path, int value)
        {
            // that's the node we've been looking for!
            if (path.IsEmpty())
            {
                LocaleValues.Add(locale, value);
                return;
            }

            // we need to go deeper
            CldrTreeNode child;
            var pathSegment = path.Dequeue();

            if (pathSegment.IsDictionaryKey)
            {
                if (PropertyChildren.TryGetValue(pathSegment.Key, out child) == false)
                {
                    child = new CldrTreeNode(Tree, this);
                    PropertyChildren.Add(pathSegment.Key, child);
                }
            }
            else
            {
                if (ArrayChildren.TryGetElement(pathSegment.Index, out child) == false)
                {
                    child = new CldrTreeNode(Tree, this);
                    ArrayChildren.Add(child);
                }
            }

            child.Add(locale, path, value);
        }



        /*
                public int FindMaxDepth(int currentMax)
        {
            if (PropertyChildren.IsNotEmpty())
                return PropertyChildren.Select(x => x.FindMaxDepth(currentMax + 1)).Max();
            return currentMax;
        }
        public IEnumerable<CldrTreeNode> FindVertices()
        {
            var vertices = new List<CldrTreeNode>() { this };
            if (PropertyChildren.IsNotEmpty())
                vertices.AddRange(PropertyChildren.SelectMany(x => x.FindVertices()));
            return vertices;
        }
        public IEnumerable<CldrTreeNode> FindValues()
        {
            var leaves = new List<CldrTreeNode>();
            if (PropertyChildren.IsEmpty())
                leaves.Add(this);
            else
                leaves.AddRange(PropertyChildren.SelectMany(x => x.FindValues()));
            return leaves;
        }
    */
    }
}