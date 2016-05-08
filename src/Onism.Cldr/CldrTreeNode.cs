using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;
using ProtoBuf;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents a queue of edges to choose while traversing a <see cref="CldrTree"/>.
    /// </summary>
    public class CldrTreePath : Queue<CldrTreePathSegment>
    {
        public static CldrTreePath Parse(string path)
        {
            var index = @"\[(?<index>[0-9]+)\]";    // [0]
            var key = @"(?<key>[a-zA-Z0-9-_%/,$\+\*]+)"; // year
            var special = @"\['(?<key>[^']+)'\]"; // ['x.x']

            var firstSegment = $@"(({index})|({key})|({special}))";
            var nextSegment = $@"(({index})|(\.{key})|({special}))*";

            var pattern = $"^{firstSegment}{nextSegment}$";

            var match = Regex.Match(path, pattern);

            var keys = match.Groups["key"].Captures.Cast<Capture>().Select(x => new {x.Index, x.Value, IsKey = true});
            var indexes = match.Groups["index"].Captures.Cast<Capture>().Select(x => new {x.Index, x.Value, IsKey = false});

            var merged = keys.Concat(indexes).OrderBy(x => x.Index);

            var result = new CldrTreePath();

            //if (!match.Success)
              //  throw new FormatException($"Path segment expected to match '{pattern}' but was '{potentialSegment}'.");

            foreach (var capture in merged)
            {
                if (capture.IsKey)
                    result.Enqueue(new CldrTreePathSegment(capture.Value));
                else
                    result.Enqueue(new CldrTreePathSegment(int.Parse(capture.Value)));
            }

            return result;
        }
    }

    /// <summary>
    /// Represents an edge to choose while traversing a <see cref="CldrTree"/>.
    /// </summary>
    public class CldrTreePathSegment
    {
        /// <summary>
        /// Gets a flag indicating whether this segment is a dictionary key (true)
        /// or an array index (false).
        /// </summary>
        public bool IsDictionaryKey { get; }

        /// <summary>
        /// Gets or sets the dictionary key used to select a proper child node
        /// while traversing the tree.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the array index used to select a proper child node
        /// while traversing the tree.
        /// </summary>
        public int Index { get; }

        public CldrTreePathSegment(string key)
        {
            this.IsDictionaryKey = true;
            this.Key = key;
        }

        public CldrTreePathSegment(int index)
        {
            this.IsDictionaryKey = false;
            this.Index = index;
        }

        public override string ToString()
        {
            return IsDictionaryKey
                ? Key
                : Index.ToString();
        }
    }

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
        public readonly Dictionary<string, CldrTreeNode> ChildrenByKeys = new Dictionary<string, CldrTreeNode>();

        [ProtoMember(2)]
        public readonly List<CldrTreeNode> ChildrenByIndexes = new List<CldrTreeNode>();

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
            var value = Tree.GetValueById(valueId);
            return value;
        }

        public override int GetHashCode()
        {
            return ChildrenByKeys.GetHashCode()
                ^ ChildrenByIndexes.GetHashCode()
                ^ LocaleValues.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrTreeNode;

            if (other == null)
                return false;

            var keyedChildrenEqual = ChildrenByKeys.IsSameAs(other.ChildrenByKeys);
            var indexedChildrenEqual = ChildrenByIndexes.IsSameAs(other.ChildrenByIndexes);
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
                    ?.ChildrenByKeys
                    ?.FirstOrDefault(x => ReferenceEquals(x.Value, this))
                    .Key;
            }
        }

        public int Index
        {
            get
            {
                var parentArray = Parent?.ChildrenByIndexes;
                
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
                if (ChildrenByKeys.TryGetValue(pathSegment.Key, out child) == false)
                    return null;
            }
            else
            {
                if (ChildrenByIndexes.TryGetElement(pathSegment.Index, out child) == false)
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
                if (ChildrenByKeys.TryGetValue(pathSegment.Key, out child) == false)
                {
                    child = new CldrTreeNode(Tree, this);
                    ChildrenByKeys.Add(pathSegment.Key, child);
                }
            }
            else
            {
                if (ChildrenByIndexes.TryGetElement(pathSegment.Index, out child) == false)
                {
                    child = new CldrTreeNode(Tree, this);
                    ChildrenByIndexes.Add(child);
                }
            }

            child.Add(locale, path, value);
        }



        /*
                public int FindMaxDepth(int currentMax)
        {
            if (ChildrenByKeys.IsNotEmpty())
                return ChildrenByKeys.Select(x => x.FindMaxDepth(currentMax + 1)).Max();
            return currentMax;
        }
        public IEnumerable<CldrTreeNode> FindVertices()
        {
            var vertices = new List<CldrTreeNode>() { this };
            if (ChildrenByKeys.IsNotEmpty())
                vertices.AddRange(ChildrenByKeys.SelectMany(x => x.FindVertices()));
            return vertices;
        }
        public IEnumerable<CldrTreeNode> FindValues()
        {
            var leaves = new List<CldrTreeNode>();
            if (ChildrenByKeys.IsEmpty())
                leaves.Add(this);
            else
                leaves.AddRange(ChildrenByKeys.SelectMany(x => x.FindValues()));
            return leaves;
        }
    */
    }
}