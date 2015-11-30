using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Onism.Cldr.Extensions;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents a single CLDR JSON file. 
    /// </summary>
    public class CldrJson
    {
        /// <summary>
        /// Gets the locale this file is associated with.
        /// </summary>
        public CldrLocale Locale { get; }

        /// <summary>
        /// Gets the token (JSON subtree) containing actual data stored in this file.
        /// </summary>
        public JProperty Data { get; }

        /// <summary>
        /// JSON files are assumed to consist exclusiely of the specified types.
        /// If any other <see cref="JTokenType"/> is found, a file is considered invalid.
        /// </summary>
        private static HashSet<JTokenType> supportedTokenTypes = new HashSet<JTokenType>
        {
            JTokenType.Object,
            JTokenType.Property,
            JTokenType.String
        };

        private CldrJson(CldrLocale locale, JProperty data)
        {
            Locale = locale;
            Data = data;
        }

        /// <summary>
        /// Loads a <see cref="CldrJson"/> from a string that contains CLDR JSON.
        /// </summary>
        /// <param name="json">A CLDR JSON represented as a string.</param>
        public static CldrJson Parse(string json)
        {
            return Parse(JObject.Parse(json));
        }

        /// <summary>
        /// Loads a <see cref="CldrJson"/> from a <see cref="JObject"/> representing
        /// a CLDR JSON file.
        /// </summary>
        /// <param name="obj">A CLDR JSON represented as a <see cref="JObject"/>.</param>
        public static CldrJson Parse(JObject obj)
        {
            CheckSupportedTypes(obj);
            
            // root - should have main (only)
            CheckContainsProperty(obj, "main", JTokenType.Object);
            CheckPropertyCount(obj, 1);

            // main - should have locale (only)
            var main = (JObject)obj["main"];
            CheckPropertyCount(main, 1);

            // en-GB
            var localeCode = main.Properties().First().Name;
            CheckContainsProperty(main, localeCode, JTokenType.Object);

            // en-GB - should have identity and data (only)
            var locale = (JObject)main[localeCode];
            CheckContainsProperty(locale, "identity", JTokenType.Object);
            CheckPropertyCount(locale, 2);

            // extract the information
            var identity = JsonConvert.DeserializeObject<CldrLocale>(locale["identity"].ToString());
            var data = locale.Properties().First(x => x.Name != "identity");

            return new CldrJson(identity, data);
        }

        /// <summary>
        /// Ensures this JObject contains a property of a specific name and value type.
        /// </summary>
        private static void CheckContainsProperty(JObject obj, string name, JTokenType valueType)
        {
            var property = obj.Properties().FirstOrDefault(x => x.Name == name);
            
            if (property == null)
                throw new FormatException($"Required property is missing: {name}.");

            if (property.Value.Type != valueType)
                throw new FormatException($"Required property value type mismatch.");
        }

        /// <summary>
        /// Ensures this JObject consists exclusively of supported types.
        /// </summary>
        private static void CheckSupportedTypes(JObject obj)
        {
            var unsupportedTypes = obj.GetAllTypes().Except(supportedTokenTypes);

            if (unsupportedTypes.IsNotEmpty())
                throw new FormatException($"Unsupported JTokenTypes used in JSON: {string.Join(", ", unsupportedTypes)}.");
        }

        /// <summary>
        /// Ensures the actual number of properties in this JObject
        /// is equal to their expected number.
        /// </summary>
        private static void CheckPropertyCount(JObject obj, int expected)
        {
            var actual = obj.Properties().Count();

            if (actual != expected)
                throw new FormatException($"Expected {expected} properties, but found {actual}.");
        }
    }
}
