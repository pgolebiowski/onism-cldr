using Newtonsoft.Json.Linq;
using NJsonSchema;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the schema provided by the derived class.
    /// </summary>
    /// <remarks>
    /// The contract assumes that any methods from this parser can be
    /// called for a certain JSON object only if in the beginning
    /// the <see cref="IsValid"/> method returned "true" for it.
    /// 
    /// The following order of operations is assumed:
    /// 
    ///     1. The <see cref="ExtractMetadata"/> method is called.
    ///     2. The <see cref="RemoveMetadata"/> method is called.
    ///     3. Subsetting is performed (externally).
    ///     4. The <see cref="PrepareForMerging"/> method is called.
    /// 
    /// </remarks>
    internal abstract class CldrJsonParser
    {
        private readonly JsonSchema4 schema;

        protected CldrJsonParser(string schema)
        {
            this.schema = JsonSchema4.FromJson(schema);
        }

        /// <summary>
        /// Determines whether the JSON object is valid against the schema of this parser.
        /// Being valid implies satisfying assumptions on the JSON structure.
        /// </summary>
        public bool IsValid(JObject obj)
        {
            var validationErrors = this.schema.Validate(obj);
            return validationErrors.IsEmpty();
        }

        /// <summary>
        /// Extracts metadata from the specified JSON object. If no metadata
        /// is provided in this type of JSONs, null is returned.
        /// </summary>
        public virtual CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return null;
        }

        /// <summary>
        /// Removes metadata from the specified JSON object. If no metadata
        /// is provided in this type of JSONs, nothing is removed.
        /// </summary>
        public virtual void RemoveMetadata(JObject obj)
        {

        }

        /// <summary>
        /// Returns a normalized <see cref="CldrJson"/> object, ready to be merged
        /// into <see cref="CldrTree"/>.
        /// </summary>
        /// <remarks>
        /// Note that the specified JSON object does not have metadata anymore.
        /// What is more, the subsetting step (3rd) might have removed some of its
        /// subtrees. It cannot be assumed to be valid against this parser's schema anymore.
        /// However, it is assumed that this JSON is the previously valid JSON,
        /// but with some of its descendants removed.
        /// </remarks>
        public virtual CldrJson PrepareForMerging(CldrLocale locale, JObject obj)
        {
            return new CldrJson(locale, obj); // by default no modifications
        }
    }
}