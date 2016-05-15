using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "DefaultContent" schema.
    /// </summary>
    internal sealed class DefaultContentParser : CldrJsonParser
    {
        public DefaultContentParser() : base(CldrJsonSchemas.DefaultContent)
        {
        }
    }
}