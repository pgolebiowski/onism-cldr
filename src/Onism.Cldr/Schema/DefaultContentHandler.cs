using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Resources;

namespace Onism.Cldr.Schema
{
    public class DefaultContentHandler : CldrJsonHandler
    {
        public DefaultContentHandler() : base(CldrJsonSchemas.DefaultContent)
        {
        }

        public override void Merge(CldrData cldrData, JToken token)
        {
            var defaultContent = token.SelectTokens("defaultContent").Children().Select(x => (string)x);
            cldrData.DefaultContent = defaultContent;
        }
    }
}