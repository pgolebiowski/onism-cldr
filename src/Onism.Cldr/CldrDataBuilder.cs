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
            var forJsonMerging = new List<JObject>();
            var forCldrTreeMerging = new List<CldrJson>();

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

                    if (handler.IncludeInCldrTree)
                        forCldrTreeMerging.Add(handler.PrepareForCldrTreeMerging(token, metadata?.CldrLocale));
                    else
                        forJsonMerging.Add(handler.PrepareForJsonMerging(token, metadata?.CldrLocale));

                    wasMatched = true;
                }

                if (!wasMatched)
                {
                    // react depending on the options (-ignore, -warning, -error)
                }
            }

            var merged = forJsonMerging.Count > 1 ? forJsonMerging.HierarchicalAggregate((a, b) =>
            {
                a.Merge(b);
                return a;
            }) : new JObject();
            
            var cldrTree = new CldrTree();
            foreach (var cldrJson in forCldrTreeMerging)
                cldrTree.Add(cldrJson);

            return new CldrData
            {
                Main = cldrTree,
                Other = merged.ToString(Formatting.None)
            };
        }
    }
}
