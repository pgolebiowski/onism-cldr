using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Onism.Cldr.Extensions;
using Onism.Cldr.Test.Utils;
using Onism.Cldr.Tools.Subsetting;

namespace Onism.Cldr.Test.Subsetting
{
    [TestFixture]
    public class TokenRemovalTest
    {
        public class TokenRemovalTestCase : TestCaseData
        {
            public TokenRemovalTestCase(string original, string expected, params string[] patterns) : base(original, expected, patterns)
            {
            }
        }

        public static IEnumerable<TokenRemovalTestCase> EmptyTokens()
        {
            yield return new TokenRemovalTestCase("{}", "{}");
            yield return new TokenRemovalTestCase("{}", "{}", "*");
            yield return new TokenRemovalTestCase("{}", "{}", "!*");
            yield return new TokenRemovalTestCase("[]", "[]", "*");
            yield return new TokenRemovalTestCase("[]", "[]", "!*");
        }

        public static IEnumerable<TokenRemovalTestCase> OnePropertyObject()
        {
            var original = @"{
                'k': 'v'
            }";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "{}", "*");
            yield return new TokenRemovalTestCase(original, original, "*", "!*");
            yield return new TokenRemovalTestCase(original, original, "k", "!k");
            yield return new TokenRemovalTestCase(original, "{}", "k");
        }

        public static IEnumerable<TokenRemovalTestCase> OneValueArray()
        {
            var original = @"[
                'v'
            ]";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "[]", "[*]");
            yield return new TokenRemovalTestCase(original, original, "[*]", "![*]");
            yield return new TokenRemovalTestCase(original, original, "[0]", "![0]");
            yield return new TokenRemovalTestCase(original, original, "[*]", "![*]");
            yield return new TokenRemovalTestCase(original, "[]", "[*]");
            yield return new TokenRemovalTestCase(original, "[]", "[0]");
        }

        public static IEnumerable<TokenRemovalTestCase> ThreePropertyObject()
        {
            var original = @"{
                'k1': 'v1',
                'k2': 'v2',
                'k3': 'v3'
            }";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "{}", "*");
            yield return new TokenRemovalTestCase(original, original, "*", "!*");
            yield return new TokenRemovalTestCase(original, original, "k*", "!k*");
            yield return new TokenRemovalTestCase(original, original, "k1", "k2", "k3", "!k1", "!k2", "!k3");
            yield return new TokenRemovalTestCase(original, "{'k2':'v2','k3':'v3'}", "k1");
            yield return new TokenRemovalTestCase(original, "{'k2':'v2','k3':'v3'}", "k1", "k2", "k3", "!k2", "!k3");
            yield return new TokenRemovalTestCase(original, "{'k2':'v2'}", "k1", "k2", "k3", "!k2");
            yield return new TokenRemovalTestCase(original, "{}", "k1", "k2", "k3");
        }

        public static IEnumerable<TokenRemovalTestCase> ThreeValueArray()
        {
            var original = @"[
                'v1',
                'v2',
                'v3'
            ]";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "[]", "[*]");
            yield return new TokenRemovalTestCase(original, original, "[*]", "![*]");
            yield return new TokenRemovalTestCase(original, original, "[0]", "[1]", "[2]", "![0]", "![1]", "![2]");
            yield return new TokenRemovalTestCase(original, "['v1','v2']", "[0]", "[1]", "[2]", "![0]", "![1]");
            yield return new TokenRemovalTestCase(original, "['v1']", "[0]", "[1]", "[2]", "![0]");
            yield return new TokenRemovalTestCase(original, "[]", "[0]", "[1]", "[2]");
        }

        public static IEnumerable<TokenRemovalTestCase> ThreeObjectArray()
        {
            var half1 = "{ 'k1': 'v1' }";
            var half2 = "{ 'k2': 'v2' }";

            var nested = @"{
                'k1': 'v1',
                'k2': 'v2'
            }";

            var original = $@"[
                {nested},
                {nested},
                {nested}
            ]";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "[]", "[*]");
            yield return new TokenRemovalTestCase(original, original, "[*]", "![*]");
            yield return new TokenRemovalTestCase(original, original, "[0]", "[1]", "[2]", "![0]", "![1]", "![2]");
            yield return new TokenRemovalTestCase(original, $"[{nested},{nested}]", "[0]", "[1]", "[2]", "![0]", "![1]");
            yield return new TokenRemovalTestCase(original, $"[{nested}]", "[0]", "[1]", "[2]", "![0]");
            yield return new TokenRemovalTestCase(original, "[]", "[0]", "[1]", "[2]");

            yield return new TokenRemovalTestCase(original, $"[{half1},{half1},{half1}]", "[*].k2");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half1},{half1}]", "[*].k2");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half1},{half1}]", "[0].k2", "[1].k2", "[2].k2");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half1},{half1}]", "[*]", "![*].k1");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half1},{half1}]", "[*]", "![0].k1", "![1].k1", "![2].k1");

            yield return new TokenRemovalTestCase(original, $"[{half2},{half2},{half2}]", "[*].k1");
            yield return new TokenRemovalTestCase(original, $"[{half2},{half2},{half2}]", "[*].k1");
            yield return new TokenRemovalTestCase(original, $"[{half2},{half2},{half2}]", "[0].k1", "[1].k1", "[2].k1");
            yield return new TokenRemovalTestCase(original, $"[{half2},{half2},{half2}]", "[*]", "![*].k2");
            yield return new TokenRemovalTestCase(original, $"[{half2},{half2},{half2}]", "[*]", "![0].k2", "![1].k2", "![2].k2");

            yield return new TokenRemovalTestCase(original, $"[{half1},{half1},{half2}]", "[*].k2", "[2].k1", "![2].k2");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half1}]", "[*].k2", "[2]");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half1}]", "[*]", "![0].k1", "![1].k1");
            yield return new TokenRemovalTestCase(original, $"[{half1},{half2}]", "[*]", "![*]", "[1]", "[2]", "[0].k2", "![1].k2");
        }

        public static IEnumerable<TokenRemovalTestCase> DeepArray()
        {
            var v = "['v']";
            var vv = $"[{v},{v}]";
            var original = $"[[[{vv}],[{vv}],[{vv}]]]";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "[]", "[*]");
            yield return new TokenRemovalTestCase(original, $"[[[{vv}],[{vv}]]]", "[0][0]");
            yield return new TokenRemovalTestCase(original, $"[[[{vv}]]]", "[0][0]", "[0][1]");
            yield return new TokenRemovalTestCase(original, "[]", "[0][0]", "[0][1]", "[0][2]");
            yield return new TokenRemovalTestCase(original, $"[[[{vv}]]]", "[*]", "![0][0]");

            yield return new TokenRemovalTestCase(original, $"[[[[{v}]],[[{v}]],[[{v}]]]]", "[0][*][0][0]");
            yield return new TokenRemovalTestCase(original, $"[[[[{v}]],[[{v}]],[{vv}]]]", "[0][*][0][0]", "![0][2][0][0]");
            yield return new TokenRemovalTestCase(original, $"[[[[{v}]],[{vv}],[{vv}]]]", "[0][*][0][0]", "![0][1][0][0]", "![0][2][0][0]");
            yield return new TokenRemovalTestCase(original, $"[[[{vv}],[{vv}],[{vv}]]]", "[0][*][0][0]", "![0][0][0][0]", "![0][1][0][0]", "![0][2][0][0]");

            yield return new TokenRemovalTestCase(original, $"[[[[{v}]],[[{v}]],[[{v}]]]]", "[0]", "![0][0]", "[0][0][0][1]", "![0][1]", "[0][1][0][1]", "![0][2]", "[0][2][0][1]");
            yield return new TokenRemovalTestCase(original, $"[[[[{v}]],[[{v}]],[[{v}]]]]", "[0]", "![0][0]", "[0][0][0][0]", "![0][1]", "[0][1][0][0]", "![0][2]", "[0][2][0][0]", "[0][0][0][0]", "[0][1][0][0]", "[0][2][0][0]", "[0][*][0][0]");
        }

        public static IEnumerable<TokenRemovalTestCase> DeepObject()
        {
            var kk = @"{
                'k1': 'v1',
                'k2': 'v2'
            }";

            var kkkk = $@"{{
                'k1': {kk},
                'k2': {kk}
            }}";

            var original = $"{{'k1': {kkkk} }}";

            yield return new TokenRemovalTestCase(original, original);
            yield return new TokenRemovalTestCase(original, "{}", "*");
            yield return new TokenRemovalTestCase(original, "{'k1':{'k1':{'k1':'v1','k2':'v2'}, 'k2':{'k1':'v1','k2':'v2'}}}", "*", "!k1.k1.k1", "!k1.k1.k2", "!k1.k2.k1", "!k1.k2.k2");
            yield return new TokenRemovalTestCase(original, "{'k1':{'k1':{'k1':'v1','k2':'v2'}, 'k2':{'k1':'v1'}}}", "*", "!k1.k1.k1", "!k1.k1.k2", "!k1.k2.k1");
            yield return new TokenRemovalTestCase(original, "{'k1':{'k1':{'k1':'v1'}, 'k2':{'k1':'v1'}}}", "*", "!k1.k1.k1", "!k1.k2.k1");
            yield return new TokenRemovalTestCase(original, "{'k1':{'k1':{'k1':'v1'}, 'k2':{'k1':'v1'}}}", "*.*.k2");
            yield return new TokenRemovalTestCase(original, "{'k1':{'k1':{'k2':'v2'}, 'k2':{'k2':'v2'}}}", "*.*.k1");
            yield return new TokenRemovalTestCase(original, "{'k1':{'k1':{'k1':'v1'}}}", "*", "!k1.k1.k1");
            yield return new TokenRemovalTestCase(original, "{}", "$..k1");
        }

        [TestCaseSource(nameof(EmptyTokens))]
        [TestCaseSource(nameof(OnePropertyObject))]
        [TestCaseSource(nameof(OneValueArray))]
        [TestCaseSource(nameof(ThreePropertyObject))]
        [TestCaseSource(nameof(ThreeValueArray))]
        [TestCaseSource(nameof(ThreeObjectArray))]
        [TestCaseSource(nameof(DeepArray))]
        [TestCaseSource(nameof(DeepObject))]
        public void HandWrittenTests(string original, string expected, string[] patterns)
        {
            // Arrange
            var originalJson = JToken.Parse(original);
            var expectedJson = JToken.Parse(expected);
            var patternCollection = PatternCollection.Parse(patterns);

            Console.WriteLine($"PATTERNS:\n{patterns.MergeLines()}\n");
            Console.WriteLine($"ORIGINAL:\n{originalJson}\n");
            Console.WriteLine($"EXPECTED:\n{expectedJson}\n");

            // Act
            originalJson.Subset(patternCollection);

            // Assert
            Console.WriteLine($"WAS:\n{originalJson}\n");
            Assert.That(JToken.DeepEquals(expectedJson, originalJson));
        }

        public static IEnumerable<TestCaseData> TestGenerationRecipes()
        {
            for (var arity = 2; arity <= 4; ++arity)
                for (var depth = 2; depth <= 6; ++depth)
                    for (var i = 0; i < 25; ++i)
                        yield return new TestCaseData(arity, depth, 50);
        }

        [TestCaseSource(nameof(TestGenerationRecipes))]
        public void GeneratedTests(int arity, int depth, int patternCount)
        {
            // Arrange
            var random = new DeterministicRandom(3000);
            var treeGenerator = new JContainerGenerator(random);
            var patternGenerator = new PatternCollectionGenerator(random);

            var original = treeGenerator.GeneratePerfectTree(arity, depth);
            var patterns = patternGenerator.GeneratePatterns(original, patternCount);

            var expected = original.DeepClone();
            var actual = original.DeepClone();

            // Act
            expected.NaiveRemove(patterns);
            actual.Subset(patterns);

            // Debug
            Console.WriteLine($"BIT ARRAY:\n{random}\n");
            //Console.WriteLine($"PATTERNS:\n{patterns.MergeLines()}\n");
            //Console.WriteLine($"ORIGINAL:\n{original.ToPrettyString()}\n");
            //Console.WriteLine($"EXPECTED:\n{expected.ToPrettyString()}\n");
            //Console.WriteLine($"WAS:\n{actual.ToPrettyString()}\n");

            // Assert
            Assert.That(JToken.DeepEquals(expected, actual));
        }
    }
}
