using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.JsonParsers;
using Onism.Cldr.Tools.Subsetting;

namespace Onism.Cldr.Tools
{
    public class CldrDataBuilder
    {
        private readonly CldrJsonFileFinder fileFinder;
        private readonly CldrVersionConsistencyAssurer versionConsistencyAssurer;
        private readonly CldrJsonParser[] jsonParsers;

        public CldrDataBuilder()
        {
            this.fileFinder = new CldrJsonFileFinder();
            this.versionConsistencyAssurer = new CldrVersionConsistencyAssurer();
            this.jsonParsers = new CldrJsonParser[]
            {
                new AvailableLocalesParser(),
                new DefaultContentParser(),
                new MainParser(),
                new RbnfParser(),
                new ScriptMetadataParser(),
                new SegmentsParser(),
                new SupplementalParser()
            };
        }

        public CldrData Build(string directory, PatternCollection patterns)
        {
            var cldrTreeBuilder = new CldrTreeBuilder();
            var done = 0;

            foreach (var path in this.fileFinder.FindFiles(directory))
            {
                var json = File.ReadAllText(path);
                var token = JObject.Parse(json);
                var wasMatched = false;

                foreach (var parser in this.jsonParsers)
                {
                    if (!parser.IsValid(token))
                        continue;

                    var metadata = parser.ExtractMetadata(token);
                    parser.RemoveMetadata(token);

                    this.versionConsistencyAssurer.AssureVersionIsConsistent(metadata?.CldrVersion, path);

                    token.Subset(patterns);

                    var toAdd = parser.PrepareForMerging(metadata?.CldrLocale, token);
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
