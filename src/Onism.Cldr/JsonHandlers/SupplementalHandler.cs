using Newtonsoft.Json.Linq;
using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class SupplementalHandler : CldrJsonHandler
    {
        public SupplementalHandler() : base(CldrJsonSchemas.Supplemental)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("supplemental.version._cldrVersion").ToString()
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("supplemental.version").Parent.Remove();
        }
    }
}
