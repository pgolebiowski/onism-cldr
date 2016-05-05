using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class DefaultContentHandler : CldrJsonHandler
    {
        public DefaultContentHandler() : base(CldrJsonSchemas.DefaultContent)
        {
        }
    }
}