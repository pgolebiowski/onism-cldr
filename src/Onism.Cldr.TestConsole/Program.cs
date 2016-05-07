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
using Onism.Cldr.JsonHandlers;
using Onism.Cldr.Subsetting;

namespace Onism.Cldr.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cldr.NET Test Console");
            Console.WriteLine("Doing stuff...");

            /*var builder = new CldrDataBuilder();
            var directory1 = @"\\Mac\Home\Downloads\jsons\standard\cldr-dates-full-master";

            var data = builder.Build(directory2, PatternCollection.Parse(""));

            data.WriteToFile(@"\\Mac\Home\Desktop\some16.bin");
            */

            var data = CldrData.LoadFromFile(@"\\Mac\Home\Desktop\some16.bin");
            var directory2 = @"\\Mac\Home\Downloads\jsons\";
            var fileFinder = new CldrJsonFileFinder();

            var jsonHandlers = new CldrJsonHandler[]
            {
                new AvailableLocalesHandler(),
                new DefaultContentHandler(),
                new MainHandler(),
                new RbnfHandler(),
                new ScriptMetadataHandler(),
                new SegmentsHandler(),
                new SupplementalHandler()
            };

            foreach (var path in fileFinder.FindFiles(directory2))
            {
                var token = JObject.Parse(File.ReadAllText(path));

                foreach (var handler in jsonHandlers)
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


            Console.WriteLine();
            Console.WriteLine("Finished");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
