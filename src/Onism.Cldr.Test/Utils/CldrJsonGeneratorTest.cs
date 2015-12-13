using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Packages;
using Ploeh.AutoFixture;

namespace Onism.Cldr.Test.Utils
{
    [TestFixture]
    public class CldrJsonGeneratorTest
    {
        private static IEnumerable<JToken> GetObjectsAndStrings(CldrJson cj)
        {
            return cj.Data.DescendantsAndSelf().Where(x => x.Type != JTokenType.Property);
        }

        [Test]
        public void GenerateOne_FullTree()
        {
            var cldrJson = CldrJsonGenerator.Generate(4, 3);

            // lvl 0: 1
            // lvl 1: 4
            // lvl 2: 16
            // lvl 3: 64
            // total: 85

            GetObjectsAndStrings(cldrJson).Count().Should().Be(85);
        }

        [Test]
        public void GenerateOne_RemoveAllButOne()
        {
            var fixture = new Fixture();
            var locale = fixture.Create<CldrLocale>().Yield();
            var cldrJsons = CldrJsonGenerator.Generate(locale, 2, 5, 0.03125, 1);

            foreach (var cldrJson in cldrJsons)
            {
                cldrJson.Data.Leaves().Count().Should().Be(1, "31/32 leaves are removed");
                GetObjectsAndStrings(cldrJson).Count().Should().Be(6, "straight line");
            }
        }

        [Test]
        public void GenerateMany_RemoveQuarterAndRefill()
        {
            const int arity = 4;
            const int depth = 3;

            var fixture = new Fixture();
            var locales = fixture.CreateMany<CldrLocale>(5);
            var cldrJsons = CldrJsonGenerator.Generate(locales, arity, depth, 0.75, 0.875);

            // counts
            foreach (var cldrJson in cldrJsons)
            {
                cldrJson.Data.Leaves().Count().Should().Be(48);
                GetObjectsAndStrings(cldrJson).Count().Should().BeInRange(64, 69);
            }

            // common
            var common = new HashSet<string>(cldrJsons.First().Data.LeafPaths());
            common.Count.Should().Be(48);

            foreach (var paths in cldrJsons.Select(x => x.Data.Leaves()))
            {
                var hashSetPaths = new HashSet<string>(paths.Select(x => x.Path));
                hashSetPaths.SequenceEqual(common).Should().BeTrue();
            }

            // refill
            foreach (var cldrJson in cldrJsons)
            {
                CldrJsonGenerator.Refill(cldrJson.Data, arity, depth);

                cldrJson.Data.Leaves().Count().Should().Be(64);
                GetObjectsAndStrings(cldrJson).Count().Should().Be(85);
            }

            // all common paths still exist
            var commonHashSet = new HashSet<string>(common);
            foreach (var cldrJson in cldrJsons)
            {
                var paths = new HashSet<string>(cldrJson.Data.LeafPaths());
                commonHashSet.IsProperSubsetOf(paths).Should().BeTrue();
            }
        }

        [Test]
        public void GenerateMany_AllCommonAllDistinct()
        {
            const int arity = 2;
            const int depth = 5;

            var fixture = new Fixture();
            var locales = fixture.CreateMany<CldrLocale>(3);
            var cldrJsons = CldrJsonGenerator.Generate(locales, arity, depth, 1, 0);

            // common
            var common = new HashSet<string>(cldrJsons.First().Data.LeafPaths());
            common.Count.Should().Be(32);

            foreach (var paths in cldrJsons.Select(x => x.Data.Leaves()))
            {
                var hashSetPaths = new HashSet<string>(paths.Select(x => x.Path));
                hashSetPaths.SequenceEqual(common).Should().BeTrue();
            }

            // all should be distinct
            cldrJsons.SelectMany(x => x.Data.Leaves()).Select(x => (string)x).Distinct().Count().Should().Be(96);
        }
    }
}
