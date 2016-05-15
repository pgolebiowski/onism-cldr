using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "ScriptMetadata" schema.
    /// </summary>
    internal sealed class ScriptMetadataParser : CldrJsonParser
    {
        public ScriptMetadataParser() : base(CldrJsonSchemas.ScriptMetadata)
        {
        }
    }
}
