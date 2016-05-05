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

        public virtual bool IncludeInCldrTree => false;

        public virtual JObject PrepareForJsonMerging(JObject obj, CldrLocale locale)
        {
            return obj; // no need for modifications
        }

        public virtual CldrJson PrepareForCldrTreeMerging(JObject obj, CldrLocale locale)
        {
            return null; // no data for merging
        }
    }
}