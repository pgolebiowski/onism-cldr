using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Tools;
using Onism.Cldr.Tools.Subsetting;

namespace Onism.Cldr.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cldr.NET Test Console");
            Console.WriteLine("Doing stuff...");

            var inputDirectory = @"\\Mac\Home\Downloads\cldr-cal-hebrew-full-master";
            var outputFile = @"\\Mac\Home\Desktop\some19.bin";

            var patterns = new PatternCollectionBuilder()
                .Exclude("*")
                .Include("main.*.dates.calendars.hebrew.months")
                .Build();

            var builder = new CldrDataBuilder();
            var data = builder.Build(inputDirectory, patterns);
            
            data.WriteToFile(outputFile);

            /*
            var data = CldrData.LoadFromFile(outputFile);
            var path = "dates.calendars.hebrew.months.format.abbreviated.1";
            var locale = new CldrLocale
            {
                Language = "en",
                Territory = "GB"
            };

            var value = data.Tree.SelectNode(path)[locale];
            Console.WriteLine(value);
            */
            // prints "Tishri"

            /*
            var directory = @"\\Mac\Home\Downloads\jsons\nonstandard";
            var binFile = @"\\Mac\Home\Desktop\some17.bin";

            var builder = new CldrDataBuilder();
            var data = builder.Build(directory, PatternCollection.Parse(""));

            //data.WriteToFile(binFile);
            // var data = CldrData.LoadFromFile(@"\\Mac\Home\Desktop\some16.bin");

            var fileFinder = new CldrJsonFileFinder();
            var jsonParsers = new CldrJsonParser[]
            {
                new AvailableLocalesParser(),
                new DefaultContentParser(),
                new MainParser(),
                new RbnfParser(),
                new ScriptMetadataParser(),
                new SegmentsParser(),
                new SupplementalParser()
            };

            foreach (var path in fileFinder.FindFiles(directory))
            {
                var token = JObject.Parse(File.ReadAllText(path));

                foreach (var handler in jsonParsers)
                {
                    if (!handler.IsValid(token))
                        continue;

                    var metadata = handler.ExtractMetadata(token);
                    handler.RemoveMetadata(token);

                    token = handler.PrepareForMerging(metadata?.CldrLocale, token).Data;

                    foreach (var pair in token.LeafPathsAndValues())
                    {
                        var tmpPath = pair.Key;
                        var tmpValue = pair.Value;

                        var foundNode = data.Tree.SelectNode(tmpPath);
                        var foundValue = foundNode[metadata?.CldrLocale ?? CldrLocale.None];

                        if (!string.Equals(tmpValue, foundValue))
                        {
                            Console.WriteLine(path);
                            Console.WriteLine(tmpPath);
                        }
                    }
                }                
            }
            
    */
            Console.WriteLine();
            Console.WriteLine("Finished");
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
