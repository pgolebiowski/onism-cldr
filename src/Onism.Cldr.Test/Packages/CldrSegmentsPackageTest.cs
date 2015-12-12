using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Onism.Cldr.Packages;

namespace Onism.Cldr.Test.Packages
{
    [TestFixture]
    public class CldrSegmentsPackageTest
    {
        #region Utils for testing

        private static readonly CldrSegmentsPackage DummyPackage = new CldrSegmentsPackage("dummy");
        private string _tempFolder;

        private static string CreateDirectory(string locale)
        {
            return Directory
                .CreateDirectory(Path.GetTempPath())
                .CreateSubdirectory(locale)
                .FullName;
        }

        [SetUp]
        public void Init()
        {
            _tempFolder = CreateDirectory("en-GB");
        }

        [TearDown]
        public void Dispose()
        {
            Directory.Delete(_tempFolder, true);
        }

        private CldrJson Parse(string json)
        {
            var tempPath = Path.Combine(_tempFolder, Guid.NewGuid() + ".json");
            File.WriteAllText(tempPath, json);
            return DummyPackage.TryParseFile(tempPath);
        }

        private CldrJson Parse(JObject obj)
        {
            var json = obj.ToString();
            return Parse(json);
        }

        private Action Parsing(JObject obj) => () => Parse(obj);

        private static JObject CorrectJson =>
        new JObject(
            new JProperty("segments", new JObject(
                CorrectLocale,
                CorrectData)));

        private static JProperty CorrectLocale =>
            new JProperty("identity", new JObject(
                new JProperty("language", "en"),
                new JProperty("territory", "GB")));

        private static JProperty CorrectData =>
            new JProperty("segmentations", new JObject(
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
                cj.Locale.Code.Should().Be("en-GB");
                cj.Locale.Language.Should().Be("en");
                cj.Locale.Territory.Should().Be("GB");
                cj.Locale.Script.Should().BeNull();
                cj.Locale.Variant.Should().BeNull();

                cj.Package.Should().Be<CldrSegmentsPackage>();

                var dataAsJObject = new JObject(CorrectData);
                JToken.DeepEquals(cj.Data, dataAsJObject).Should().BeTrue();
            }
        }

        [Test]
        public void TryParseFile_FullLocaleDeserialization()
        {
            // this is exception to what is set up for all the tests
            // thus dispose and change the path to be disposed once again later
            Dispose();
            _tempFolder = CreateDirectory("en-Latn-GB-POSIX");

            var o = CorrectJson;
            o["segments"]["identity"]["script"] = "Latn";
            o["segments"]["identity"]["variant"] = "POSIX";

            var cj = Parse(o);

            cj.Locale.Language.Should().Be("en");
            cj.Locale.Territory.Should().Be("GB");
            cj.Locale.Script.Should().Be("Latn");
            cj.Locale.Variant.Should().Be("POSIX");
        }

        [Test]
        public void TryParseFile_UnsupportedTokenTypes_FormatExceptionThrown()
        {
            var o = CorrectJson;
            o["segments"]["segmentations"]["4"]["C"]["iii"] = 5;
            o["segments"]["segmentations"]["5"] = null;
            o["segments"]["segmentations"]["6"] = true;

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Unsupported JTokenTypes used in JSON: Integer, Null, Boolean.");
        }

        [Test]
        public void TryParseFile_SegmentsTokenMissing_ExceptionThrown()
        {
            var o = new JObject();

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 0.");
        }

        [Test]
        public void TryParseFile_SegmentsTokenAlternative_ExceptionThrown()
        {
            var o = new JObject(
            new JProperty("alternative", new JObject(
                CorrectLocale,
                CorrectData)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property is missing: segments.");
        }

        [Test]
        public void TryParseFile_SegmentsTokenSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 2.");
        }

        [Test]
        public void TryParseFile_SegmentsTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject { ["segments"] = "<3" };

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected Object type, but found String.");
        }

        [Test]
        public void TryParseFile_IdentityTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject(
            new JProperty("segments", new JObject(
                new JProperty("identity", "<3"),
                CorrectData)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected Object type, but found String.");
        }

        [Test]
        public void TryParseFile_IdentityTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
            new JProperty("segments", new JObject(
                CorrectData)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 1.");
        }

        [Test]
        public void TryParseFile_SegmentationsTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
            new JProperty("segments", new JObject(
                CorrectLocale)));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 1.");
        }

        [Test]
        public void TryParseFile_IdentityAndSegmentationTokensSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["segments"]["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 3.");
        }

        [Test]
        public void TryParseText_NotJObject_ExceptionThrown()
        {
            var array = new JArray();
            Action action = () => Parse(array.ToString());

            action.ShouldThrow<JsonReaderException>();
        }
    }
}
