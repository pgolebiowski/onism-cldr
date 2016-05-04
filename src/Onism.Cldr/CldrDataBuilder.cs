using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Onism.Cldr.JsonHandlers;
using Onism.Cldr.Utils;

namespace Onism.Cldr
{
    public class CldrDataBuilder
    {
        private readonly CldrJsonFileFinder fileFinder;
        private readonly CldrVersionConsistencyAssurer versionConsistencyAssurer;
        private readonly CldrJsonHandler[] jsonHandlers;

        public CldrDataBuilder()
        {
            this.fileFinder = new CldrJsonFileFinder();
            this.versionConsistencyAssurer = new CldrVersionConsistencyAssurer();
            this.jsonHandlers = new CldrJsonHandler[]
            {
                new AvailableLocalesHandler(),
                new DefaultContentHandler()
            };
        }

        public CldrData Build(string directory)
        {
            var result = new CldrData();

            foreach (var file in fileFinder.FindFiles(directory))
            {
                var json = File.ReadAllText(file);
                var token = JToken.Parse(json);

                foreach (var handler in this.jsonHandlers)
                {
                    if (handler.IsValid(token))
                    {
                        // should do before something like:
                        // versionConsistencyAssurer.AssureVersionIsConsistent();

                        handler.Merge(result, token);
                        break;

                        // should go to next file
                    }
                }

                // TODO: if not valid against any schema:
                // react depending on the options (-ignore, -warning, -error)
            }

            return result;
        }
    }
}
