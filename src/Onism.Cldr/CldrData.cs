using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
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
        /// <see cref="CldrTree"/> object containing main and rbnf CLDR data.
        /// </summary>
        [ProtoMember(1)]
        public CldrTree Main { get; set; }

        /// <summary>
        /// Serialized JSON object containing all the other CLDR data: supplemental,
        /// available locales, default content, and more.
        /// </summary>
        [ProtoMember(2)]
        public string Other { get; set; }

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
