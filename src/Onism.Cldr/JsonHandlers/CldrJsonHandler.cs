using Newtonsoft.Json.Linq;
using NJsonSchema;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.JsonHandlers
{
    public abstract class CldrJsonHandler
    {
        private readonly JsonSchema4 schema;

        protected CldrJsonHandler(string schema)
        {
            this.schema = JsonSchema4.FromJson(schema);
        }

        public bool IsValid(JObject obj)
        {
            var validationErrors = this.schema.Validate(obj);
            return validationErrors.IsEmpty();
        }

        public virtual CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return null; // no metadata
        }

        public virtual void RemoveMetadata(JObject obj)
        {
        }

        public virtual CldrJson PrepareForMerging(CldrLocale locale, JObject obj)
        {
            return new CldrJson(locale, obj); // no modifications
        }
    }
}