using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using Onism.Cldr.Extensions;
using Onism.Cldr.Resources;

namespace Onism.Cldr.Schema
{
    public class CldrJsonTypeDetector
    {
        private static readonly Dictionary<JsonSchema4, CldrJsonType> Matching
            = new Dictionary<JsonSchema4, CldrJsonType>
            {
                // sorted by chance of occurrence
                {JsonSchema4.FromJson(CldrJsonSchemas.Main), CldrJsonType.Main},
                {JsonSchema4.FromJson(CldrJsonSchemas.Rbnf), CldrJsonType.Rbnf},
                {JsonSchema4.FromJson(CldrJsonSchemas.Supplemental), CldrJsonType.Supplemental},
                {JsonSchema4.FromJson(CldrJsonSchemas.Segments), CldrJsonType.Segments},
                {JsonSchema4.FromJson(CldrJsonSchemas.AvailableLocales), CldrJsonType.AvailableLocales},
                {JsonSchema4.FromJson(CldrJsonSchemas.DefaultContent), CldrJsonType.DefaultContent},
                {JsonSchema4.FromJson(CldrJsonSchemas.ScriptMetadata), CldrJsonType.ScriptMetadata}
            };

        public CldrJsonType Detect(JToken token)
        {
            foreach (var matching in Matching)
            {
                var schema = matching.Key;
                var type = matching.Value;

                var validationErrors = schema.Validate(token);
                if (validationErrors.IsEmpty())
                {
                    return type;
                }
            }

            return CldrJsonType.NotRecognized;
        }
    }
}
