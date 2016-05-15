using System.Collections;
using System.Collections.Generic;
using Onism.Cldr.Extensions;
using ProtoBuf;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents a collection of keys and their unique identifiers.
    /// </summary>
    [ProtoContract]
    internal class IdentifierDictionary<T> : IReadOnlyCollection<T>
    {
        [ProtoMember(1)]
        private readonly Dictionary<T, int> identifiers;

        [ProtoMember(2)]
        private int nextIdentifier;

        public IdentifierDictionary()
        {
            this.identifiers = new Dictionary<T, int>();
            this.nextIdentifier = 0;
        }

        /// <summary>
        /// Gets the id associated with the specified key. If the key is missing,
        /// it is assigned the next id in increasing order.
        /// </summary>
        public int GetId(T key)
        {
            int id;

            if (this.identifiers.TryGetValue(key, out id))
                return id;

            id = this.nextIdentifier++;
            this.identifiers.Add(key, id);
            return id;
        }

        public int Count => this.identifiers.Count;

        public IEnumerator<T> GetEnumerator() => this.identifiers.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object obj)
        {
            var other = obj as IdentifierDictionary<T>;
            return other != null && this.Equals(other);
        }

        protected bool Equals(IdentifierDictionary<T> other) => this.identifiers.IsSameAs(other.identifiers);

        public override int GetHashCode() => this.identifiers.GetHashCode();
    }
}