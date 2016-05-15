using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBuf;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents unified CLDR data.
    /// </summary>
    [ProtoContract]
    public class CldrData
    {
        /// <summary>
        /// Gets a collection of locales available within this <see cref="CldrData"/> instance.
        /// </summary>
        public IReadOnlyList<CldrLocale> AvailableLocales => this.Tree.Locales.ToArray();

        /// <summary>
        /// <see cref="CldrTree"/> object containing CLDR data.
        /// </summary>
        [ProtoMember(1)]
        public CldrTree Tree { get; set; }

        /// <summary>
        /// Traverses the CLDR tree using a path and gets the value
        /// associated with the specified locale.
        /// </summary>
        public string GetValue(string path, CldrLocale locale)
        {
            return this.Tree.SelectNode(path).GetValue(locale);
        }

        #region Serialization

        /// <summary>
        /// Writes a binary representation of this <see cref="CldrData"/> to the specified file.
        /// </summary>
        /// <param name="path">A relative or absolute path to the file where data is to be serialized to.</param>
        public void WriteToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
                WriteToStream(stream);
        }

        /// <summary>
        /// Writes a binary representation of this <see cref="CldrData"/> to the supplied stream.
        /// </summary>
        /// <param name="destination">The destination stream to write to.</param>
        public void WriteToStream(Stream destination)
        {
            Serializer.Serialize(destination, this);
        }

        /// <summary>
        /// Creates a new <see cref="CldrData"/> instance from a binary file.
        /// </summary>
        /// <param name="path">A relative or absolute path to the file where data is to be deserialized from.</param>
        public static CldrData LoadFromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
                return LoadFromStream(stream);
        }

        /// <summary>
        /// Creates a new <see cref="CldrData"/> instance from a binary stream.
        /// </summary>
        /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
        public static CldrData LoadFromStream(Stream source)
        {
            return Serializer.Deserialize<CldrData>(source);
        }

        #endregion
    }
}
