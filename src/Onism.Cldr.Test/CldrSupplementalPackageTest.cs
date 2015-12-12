using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using FluentAssertions;
using Onism.Cldr.Packages;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrSupplementalPackageTest
    {
        #region Utils for testing

        private static readonly CldrSupplementalPackage DummyPackage = new CldrSupplementalPackage("dummy");
        private string _tempFile;

        [SetUp]
        public void Init()
        {
            _tempFile = Path.GetTempFileName();
        }

        [TearDown]
        public void Dispose()
        {
            File.Delete(_tempFile);
        }

        private CldrJson Parse(string json)
        {
            File.WriteAllText(_tempFile, json);
            return DummyPackage.TryParseFile(_tempFile);
        }

        private CldrJson Parse(JObject obj)
        {
            var json = obj.ToString();
            return Parse(json);
        }

        private Action Parsing(JObject obj) => () => Parse(obj);

        private static JObject CorrectJson =>
            new JObject(
                new JProperty("supplemental", new JObject(
                    CorrectVersion,
                    CorrectData)));

        private  static JProperty CorrectVersion =>
            new JProperty("version", new JObject(
                new JProperty("_cldrVersion", "28"),
                new JProperty("_unicodeVersion", "8.0.0"),
                new JProperty("_number", "$Revision: 11969 $")));

        private static JProperty CorrectData =>
            new JProperty("data", new JObject(
                new JProperty("1", "aaa"),
                new JProperty("2", "bbb"),
                new JProperty("3", "ccc"),
                new JProperty("4", new JObject(
                    new JProperty("A", "ddd"),
                    new JProperty("B", "eee"),
                    new JProperty("C", new JObject(
                        new JProperty("i", "fff"),
                        new JProperty("ii", "ggg"))),
                    new JProperty("D", new JObject(
                        new JProperty("i", "hhh"),
                        new JProperty("ii", "iii")))))));

        #endregion

        [Test]
        public void TryParseFile_CorrectInput()
        {
            var o = CorrectJson;

            var fromObject = Parse(o);
            var fromString = Parse(o.ToString());

            var cjs = new[] { fromObject, fromString };

            foreach (var cj in cjs)
            {
                cj.Locale.Code.Should().Be("none");
                cj.Locale.Territory.Should().BeNull();
                cj.Locale.Script.Should().BeNull();
                cj.Locale.Variant.Should().BeNull();

                cj.Package.Should().Be<CldrSupplementalPackage>();

                var withoutVersion = new JObject(
                new JProperty("supplemental", new JObject(
                    CorrectData)));

                JToken.DeepEquals(cj.Data, withoutVersion).Should().BeTrue();
            }
        }

        [Test]
        public void TryParseFile_UnsupportedTokenTypes_FormatExceptionThrown()
        {
            var o = CorrectJson;
            o["supplemental"]["data"]["4"]["C"]["iii"] = 5;
            o["supplemental"]["data"]["5"] = null;
            o["supplemental"]["data"]["6"] = true;

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Unsupported JTokenTypes used in JSON: Integer, Null, Boolean.");
        }

        [Test]
        public void TryParseFile_RootTokenMissing_ExceptionThrown()
        {
            var o = new JObject();

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 0.");
        }

        [Test]
        public void TryParseFile_RootTokenAlternative_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("alternative", new JObject(
                    CorrectVersion,
                    CorrectData)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property is missing: supplemental.");
        }

        [Test]
        public void TryParseFile_RootTokenSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 2.");
        }

        [Test]
        public void TryParseFile_RootTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject { ["supplemental"] = "<3" };

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected Object type, but found String.");
        }

        [Test]
        public void TryParseFile_VersionTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("supplemental", new JObject(
                    CorrectData)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 1.");
        }

        [Test]
        public void TryParseFile_VersionTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("supplemental", new JObject(
                    new JProperty("version", "<3"),
                    CorrectData)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected Object type, but found String.");
        }

        [Test]
        public void TryParseFile_DataTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("supplemental", new JObject(
                    CorrectVersion)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 1.");
        }

        [Test]
        public void TryParseFile_VersionAndDataTokensSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["supplemental"]["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 3.");
        }

        [Test]
        public void TryParseFile_NotJObject_ExceptionThrown()
        {
            var array = new JArray();
            Action action = () => Parse(array.ToString());

            action.ShouldThrow<JsonReaderException>();
        }

        [Test]
        public void TryParseFile_AvailableLocales()
        {
            var o = new JObject(
                new JProperty("availableLocales", new JObject(
                    new JProperty("blah", "BLAH"))));

            var cj = Parse(o);

            cj.Locale.Code.Should().Be("none");
            cj.Package.Should().Be<CldrSupplementalPackage>();
            JToken.DeepEquals(cj.Data, o).Should().BeTrue();
        }

        [Test]
        public void TryParseFile_DefaultContent()
        {
            var o = new JObject(
                new JProperty("defaultContent", new JObject(
                    new JProperty("blah", "BLAH"))));

            var cj = Parse(o);

            cj.Locale.Code.Should().Be("none");
            cj.Package.Should().Be<CldrSupplementalPackage>();
            JToken.DeepEquals(cj.Data, o).Should().BeTrue();
        }
    }
}
