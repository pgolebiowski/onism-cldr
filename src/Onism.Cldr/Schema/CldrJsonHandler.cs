using Newtonsoft.Json.Linq;
using NJsonSchema;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Schema
{
    public class CldrJsonHandler
    {
        private readonly JsonSchema4 schema;

        protected CldrJsonHandler(string schema)
        {
            this.schema = JsonSchema4.FromJson(schema);
        }

        public bool IsValid(JToken token)
        {
            var validationErrors = this.schema.Validate(token);
            return validationErrors.IsEmpty();
        }

        public virtual void Merge(CldrData cldrData, JToken token)
        {
            
        }
    }
}