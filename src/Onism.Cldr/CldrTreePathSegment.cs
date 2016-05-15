namespace Onism.Cldr
{
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
            return this.IsDictionaryKey
                ? this.Key
                : this.Index.ToString();
        }
    }
}