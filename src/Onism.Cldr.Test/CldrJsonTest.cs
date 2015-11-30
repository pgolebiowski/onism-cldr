using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using FluentAssertions;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrJsonTest
    {
        #region Utils for testing

        private static Action Parsing(JObject obj) => () => CldrJson.Parse(obj);

        private static JObject CorrectJson =>
            new JObject(
                new JProperty("main", new JObject(
                    new JProperty("en-GB", new JObject(
                        CorrectLocale,
                        CorrectData)))));

        private static JProperty CorrectLocale =>
            new JProperty("identity", new JObject(
                new JProperty("language", "en"),
                new JProperty("territory", "GB")));

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
        public void Parse_CorrectInput()
        {
            var o = CorrectJson;
            
            var fromObject = CldrJson.Parse(o);
            var fromString = CldrJson.Parse(o.ToString());

            var cjs = new CldrJson[] { fromObject, fromString };
            foreach (var cj in cjs)
            {
                cj.Locale.Code.Should().Be("en-GB");
                cj.Locale.Language.Should().Be("en");
                cj.Locale.Territory.Should().Be("GB");
                cj.Locale.Script.Should().BeNull();
                cj.Locale.Variant.Should().BeNull();
                JToken.DeepEquals(cj.Data, CorrectData).Should().BeTrue();
            }
        }

        [Test]
        public void Parse_FullLocaleDeserialization()
        {
            var o = CorrectJson;
            o["main"]["en-GB"]["identity"]["script"] = "Latn";
            o["main"]["en-GB"]["identity"]["variant"] = "POSIX";

            var cj = CldrJson.Parse(o);

            cj.Locale.Language.Should().Be("en");
            cj.Locale.Territory.Should().Be("GB");
            cj.Locale.Script.Should().Be("Latn");
            cj.Locale.Variant.Should().Be("POSIX");
        }

        [Test]
        public void Parse_UnsupportedTokenTypes_FormatExceptionThrown()
        {
            var o = CorrectJson;
            o["main"]["en-GB"]["data"]["4"]["C"]["iii"] = new JArray { "x", "y", "z" };
            o["main"]["en-GB"]["data"]["5"] = 5;
            o["main"]["en-GB"]["data"]["6"] = null;

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Unsupported JTokenTypes used in JSON: Array, Integer, Null.");
        }

        [Test]
        public void Parse_MainTokenMissing_ExceptionThrown()
        {
            var o = new JObject();

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property is missing: main.");
        }

        [Test]
        public void Parse_MainTokenAlternative_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("alternative", new JObject(
                    new JProperty("en-GB", new JObject(
                        CorrectLocale,
                        CorrectData)))));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property is missing: main.");
        }

        [Test]
        public void Parse_MainTokenSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 2.");
        }

        [Test]
        public void Parse_MainTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject();
            o["main"] = "<3";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property value type mismatch.");
        }

        [Test]
        public void Parse_LocaleTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("main", new JObject()));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 0.");
        }

        [Test]
        public void Parse_LocaleTokenSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["main"]["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 1 properties, but found 2.");
        }

        [Test]
        public void Parse_LocaleTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("main", new JObject(
                    new JProperty("en-GB", "<3"))));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property value type mismatch.");
        }

        [Test]
        public void Parse_IdentityTokenValueTypeMismatch_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("main", new JObject(
                    new JProperty("en-GB", new JObject(
                        new JProperty("identity", "<3"),
                        CorrectData)))));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property value type mismatch.");
        }

        [Test]
        public void Parse_IdentityTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("main", new JObject(
                    new JProperty("en-GB", new JObject(
                        CorrectData)))));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Required property is missing: identity.");
        }

        [Test]
        public void Parse_DataTokenMissing_ExceptionThrown()
        {
            var o = new JObject(
                new JProperty("main", new JObject(
                    new JProperty("en-GB", new JObject(
                        CorrectLocale)))));

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 1.");
        }

        [Test]
        public void Parse_IdentityAndDataTokensSibling_ExceptionThrown()
        {
            var o = CorrectJson;
            o["main"]["en-GB"]["sibling"] = "Nefarious Sibling";

            Parsing(o)
                .ShouldThrow<FormatException>()
                .WithMessage("Expected 2 properties, but found 3.");
        }

        [Test]
        public void ParseText_NotJObject_ExceptionThrown()
        {
            var array = new JArray();
            Action action = () => CldrJson.Parse(array.ToString());

            action.ShouldThrow<JsonReaderException>();
        }
    }
}