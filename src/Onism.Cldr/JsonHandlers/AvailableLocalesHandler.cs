using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class AvailableLocalesHandler : CldrJsonHandler
    {
        public AvailableLocalesHandler() : base(CldrJsonSchemas.AvailableLocales)
        {
        }
    }
}