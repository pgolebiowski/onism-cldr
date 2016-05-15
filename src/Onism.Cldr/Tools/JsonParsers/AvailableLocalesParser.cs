using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "AvailableLocales" schema.
    /// </summary>
    internal sealed class AvailableLocalesParser : CldrJsonParser
    {
        public AvailableLocalesParser() : base(CldrJsonSchemas.AvailableLocales)
        {
        }
    }
}