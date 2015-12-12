﻿using System.IO;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using Onism.Cldr.Extensions;
using Onism.Cldr.Test.Utils;
using Ploeh.AutoFixture;
using ProtoBuf;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrTreeTest
    {
        [Test]
        public void Add_OneAndSmall()
        {
            var cj = CldrJsonGenerator.Generate(3, 3);
            var tree = new CldrTree();

            tree.Add(cj);

            var pathValuePairs = cj
                .Data
                .Leaves()
                .Select(x => new {Path = x.Path, Value = (string) x});

            foreach (var pair in pathValuePairs)
            {
                tree.SelectNode(pair.Path)[cj.Locale].Should().Be(pair.Value);
            }
        }

        [Test]
        public void Add_ManyAndBig()
        {
            var fixture = new Fixture();
            var locales = fixture.CreateMany<CldrLocale>(5);
            var cjs = CldrJsonGenerator.Generate(locales, 5, 5, 0.75, 0.875);
            var tree = new CldrTree();

            cjs.ForEach(x => tree.Add(x));

            var localePathsPairs = cjs
                .Select(x => new {Locale = x.Locale, Paths = x.Data.Leaves().Select(y => new { Path = y.Path, Value = (string)y })});

            foreach (var pair in localePathsPairs)
                foreach (var path in pair.Paths)
                {
                    tree.SelectNode(path.Path)[pair.Locale].Should().Be(path.Value);
                }
        }

        [Test]
        public void Serialization()
        {
            var fixture = new Fixture();
            var locales = fixture.CreateMany<CldrLocale>(5);
            var cjs = CldrJsonGenerator.Generate(locales, 5, 5, 0.75, 0.875);
            var tree = new CldrTree();
            CldrTree deserialized;

            cjs.ForEach(x => tree.Add(x));

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, tree);

                using (var stream2 = new MemoryStream(stream.ToArray()))
                    deserialized = Serializer.Deserialize<CldrTree>(stream2);
            }

            tree.Equals(deserialized).Should().BeTrue();

            // let's check some really small bit
            var node = deserialized.Root;

            for (var i = 0; i < 5; ++i)
                node = node.Children.First().Value;

            var last = node.LocaleValues.Last();
            node.LocaleValues.Remove(last.Key);
            node.LocaleValues.Add(last.Key, last.Value + 1);

            tree.Equals(deserialized).Should().BeFalse();
        }
    }
}