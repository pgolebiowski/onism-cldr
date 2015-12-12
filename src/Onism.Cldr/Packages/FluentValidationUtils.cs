using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Packages
{
    /// <summary>
    /// Provides various extension methods to be used for the validation of CLDR JSON files. It supports
    /// chaining, so multiple assertions can form a single statement.
    /// </summary>
    internal static class FluentValidationUtils
    {
        /// <summary>
        /// Ensures this JObject contains a property of a specific name and value type.
        /// </summary>
        public static JObject PropertiesShouldContain(this JObject obj, string name, JTokenType valueType)
        {
            var property = obj.Properties().FirstOrDefault(x => x.Name == name);

            if (property == null)
                throw new FormatException($"Required property is missing: {name}.");

            property.Value.TokenTypeShouldBe(valueType);

            return obj;
        }

        /// <summary>
        /// Ensures the number of properties in this JObject is as expected.
        /// </summary>
        public static JObject PropertiesCountShouldBe(this JObject obj, int expected)
        {
            var actual = obj.Properties().Count();

            if (actual != expected)
                throw new FormatException($"Expected {expected} properties, but found {actual}.");

            return obj;
        }

        /// <summary>
        /// Ensures the code of this CldrLocale is as expected.
        /// </summary>
        public static CldrLocale LocaleCodeShouldBe(this CldrLocale obj, string localeCode)
        {
            if (obj.Code != localeCode)
                throw new FormatException($"Expected {localeCode} but found {obj.Code} code.");

            return obj;
        }

        /// <summary>
        /// Ensures this JToken is of a certain type.
        /// </summary>
        public static JToken TokenTypeShouldBe(this JToken token, JTokenType type)
        {
            if (token.Type != type)
                throw new FormatException($"Expected {type} type, but found {token.Type}.");

            return token;
        }

        /// <summary>
        /// Ensures this JObject consists exclusively of certain types.
        /// </summary>
        public static JObject DescendantsTypesShouldOnlyBe(this JObject obj, params JTokenType[] tokenTypes)
        {
            var unsupportedTypes = obj.GetAllTypes().Except(tokenTypes).ToArray();

            if (unsupportedTypes.IsNotEmpty())
                throw new FormatException($"Unsupported JTokenTypes used in JSON: {string.Join(", ", unsupportedTypes)}.");

            return obj;
        }
    }
}