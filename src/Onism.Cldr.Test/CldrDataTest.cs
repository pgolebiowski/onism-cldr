using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Packages;
using Ploeh.AutoFixture;
using static Onism.Cldr.Test.Utils.CldrJsonGenerator;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrDataTest
    {
        private static string GetTempDirectory()
        {
            var tempPath = Path.GetTempPath();
            var resultPath = Path.Combine(tempPath, Path.GetRandomFileName());
            Directory.CreateDirectory(resultPath);
            return resultPath;
        }

        private string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = GetTempDirectory();
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_tempDirectory, true);
        }

        [Test]
        public void MergePackages_LargeTest()
        {
            //
            // CREATE 300 CLDR JSONs
            //
            var fixture = new Fixture();
            var locales = fixture.CreateMany<CldrLocale>(20).ToArray();

            var standard = new List<CldrJson>();
            var supplemental = new List<CldrJson>();
            var segments = new List<CldrJson>();

            for (var i = 0; i < 5; ++i)
            {
                standard.AddRange(GenerateMany(typeof (CldrStandardPackage), locales, 4, 3, 0.75, 1, true));
                supplemental.AddRange(GenerateMany(typeof(CldrSupplementalPackage), locales, 2, 6, 0, 1));
                segments.AddRange(GenerateMany(typeof(CldrSegmentsPackage), locales, 3, 4, 0, 1));
            }

            //
            // SERIALIZE
            //
            var saveList = new Action<string, List<CldrJson>>((name, list) =>
            {
                var path = Path.Combine(_tempDirectory, name + CldrPackage.Extension);
                File.WriteAllText(path, JsonConvert.SerializeObject(list));
            });

            saveList("standard", standard);
            saveList("supplemental", supplemental);
            saveList("segments", segments);

            //
            // MERGE PACKAGES
            //
            var cldrData = CldrData.MergePackages(_tempDirectory);

            //
            // ALL SHOULD EXIST
            //
            var allShouldExist = new Action<CldrData>(cd =>
            {
                foreach (var cldrJson in standard)
                    foreach (var leafData in cldrJson.Data.LeafPathsAndValues())
                        cd.Standard.SelectNode(leafData.Key)[cldrJson.Locale].Should().Be(leafData.Value);

                var deserializedSuplemental = JObject.Parse(cd.Supplemental);
                foreach (var cldrJson in supplemental)
                    foreach (var leafData in cldrJson.Data.LeafPathsAndValues())
                        ((string)deserializedSuplemental.SelectToken(leafData.Key))
                            .Should().Be(leafData.Value);

                var deserializedSegments = JObject.Parse(cd.Segments);
                foreach (var cldrJson in segments)
                    foreach (var leafData in cldrJson.Data.LeafPathsAndValues())
                        ((string)deserializedSegments.SelectToken(leafData.Key))
                            .Should().Be(leafData.Value);

                // ok, so all the items are there. Just to make sure that in the standard-type data
                // there is an expected number of values and locales
                cd.Standard.Locales.Count.Should().Be(20);
                cd.Standard.Values.Count.Should().Be(5 * (16 * 20 + 48));
            });

            allShouldExist(cldrData);

            //
            // SERIALIZE CLDR DATA
            //
            var tempFile = Path.Combine(_tempDirectory, "cldrData.bin");
            cldrData.WriteToFile(tempFile);
            var deserializedCldrData = CldrData.LoadFromFile(tempFile);

            //
            // TEST ISOMORPHISM
            //
            allShouldExist(deserializedCldrData);

            var a = cldrData;
            var b = deserializedCldrData;
            a.Segments.Should().Be(b.Segments);
            a.Supplemental.Should().Be(b.Supplemental);
            a.Standard.Should().Be(b.Standard);
        }
    }
}
