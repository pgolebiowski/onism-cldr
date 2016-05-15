using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.JsonHandlers;
using Onism.Cldr.Subsetting;

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
                new DefaultContentHandler(),
                new MainHandler(),
                new RbnfHandler(),
                new ScriptMetadataHandler(),
                new SegmentsHandler(),
                new SupplementalHandler()
            };
        }

        public CldrData Build(string directory, PatternCollection patterns)
        {
            var cldrTreeBuilder = new CldrTreeBuilder();
            var other = new JObject();
            int done = 0;

            foreach (var path in fileFinder.FindFiles(directory))
            {
                var json = File.ReadAllText(path);
                var token = JObject.Parse(json);
                var wasMatched = false;

                foreach (var handler in this.jsonHandlers)
                {
                    if (!handler.IsValid(token))
                        continue;

                    var metadata = handler.ExtractMetadata(token);
                    handler.RemoveMetadata(token);

                    versionConsistencyAssurer.AssureVersionIsConsistent(metadata?.CldrVersion, path);

                    token.Subset(patterns);

                    var toAdd = handler.PrepareForMerging(metadata?.CldrLocale, token);
                    cldrTreeBuilder.Add(toAdd);
                    
                    wasMatched = true;
                }

                if (!wasMatched)
                {
                    // react depending on the options (-ignore, -warning, -error)
                }
                done++;
                if (done % 100 == 0)
                Console.WriteLine($"{done} files processed");
            }
            if (done % 100 != 0)
                Console.WriteLine($"{done} files processed");

            return new CldrData
            {
                Tree = cldrTreeBuilder.Tree
            };
        }
    }
}
