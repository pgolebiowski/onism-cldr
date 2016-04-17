using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Onism.Cldr.Extensions;
using Onism.Cldr.Schema;
using Onism.Cldr.Test.Schema.Resources;

namespace Onism.Cldr.Test.Schema
{
    [TestFixture]
    public class JsonSchemaTest
    {
        #region Fixed functional tests

        [TestCase("{ }", ExpectedResult = true)]
        [TestCase("{ 'a': '' }", ExpectedResult = false)]
        public bool EmptySchema(string json)
        {
            return JsonSchema.Parse("{ }").Validate(json);
        }

        [TestCase("{ }", ExpectedResult = false)]
        [TestCase("{ 'required': 'text' }", ExpectedResult = true)]
        [TestCase("{ 'different': '' }", ExpectedResult = false)]
        [TestCase("{ 'required': '', 'another': '' }", ExpectedResult = false)]
        public bool KnownAndRequired(string json)
        {
            return JsonSchema.Parse("{ 'required': 'STRING whatever' }").Validate(json);
        }

        [TestCase("{ }", ExpectedResult = true)]
        [TestCase("{ 'known': 'text' }", ExpectedResult = true)]
        [TestCase("{ 'different': '' }", ExpectedResult = false)]
        [TestCase("{ 'known': '', 'another': '' }", ExpectedResult = false)]
        public bool KnownAndOptional(string json)
        {
            return JsonSchema.Parse("{ '~known': 'STRING whatever' }").Validate(json);
        }

        [TestCase("{ }", false)]
        [TestCase("{ 'property': '' }", true)]
        [TestCase("{ 'one': '', 'another': '' }", false)]
        public void UnknownAndRequired(string json, bool expectedResult)
        {
            Dictionary<string, dynamic> variables;
            var isValid = JsonSchema.Parse("{ '@optional': 'STRING whatever' }").TryExtractVariables(json, out variables);
            
            Assert.AreEqual(isValid, expectedResult);

            if (isValid)
            {
                Assert.That(variables.Count.Equals(1));
                Assert.That(variables, Contains.Item(new KeyValuePair<string, string>("optional", "property")));
            }
        }

        [TestCase("{ 'property': 'text' }", ExpectedResult = true)]
        [TestCase("{ 'property': { } }", ExpectedResult = false)]
        [TestCase("{ 'property': [ 'text' ] }", ExpectedResult = false)]
        [TestCase("{ 'property': null }", ExpectedResult = false)]
        public bool ShouldNotPassWhenUnexpectedTypeIsFound(string json)
        {
            return JsonSchema.Parse("{ 'property': 'STRING whatever' }").Validate(json);
        }

        #endregion

        #region Tests enumerating through schema variants

        private static IEnumerable<string> SchemasFromResources => new[]
        {
            JsonSchemas.Diversified,
            JsonSchemas.RequiredOnly,
            JsonSchemas.NestedWithAllOptionalAndVariable
        };

        [TestCaseSource(nameof(SchemasFromResources))]
        public void ShouldPassWhenOptionalPropertiesAreMissing(string jsonSchema)
        {
            // Arrange
            var schema = JsonSchema.Parse(jsonSchema);

            foreach (var schemaVariant in EnumerateSchemaVariantsWithAllSubsetsOfOptionalProperties(jsonSchema))
            {
                var expectedVariables = new Dictionary<string, dynamic>();
                var jsonVariant = CreateJsonForSchema(schemaVariant, ref expectedVariables);

                // Act
                var actualVariables = schema.ExtractVariables(jsonVariant);

                // Assert
                var expected = expectedVariables.Select(x => $"key: {x.Key}, value: {x.Value}").ToArray();
                var actual = actualVariables.Select(x => $"key: {x.Key}, value: {x.Value}").ToArray();

                var differences = expected.Except(actual);
                Assert.That(expected.Count, Is.EqualTo(actual.Length));
                Assert.That(!differences.Any());
            }
        }

        [TestCaseSource(nameof(SchemasFromResources))]
        public void ShouldNotPassWhenRequiredPropertiesAreMissing(string jsonSchema)
        {
            // Arrange
            var schema = JsonSchema.Parse(jsonSchema);

            foreach (var schemaVariant in EnumerateSchemaVariantsWithMissingRequiredProperties(jsonSchema))
            {
                var dummyDictionary = new Dictionary<string, dynamic>();
                var jsonVariant = CreateJsonForSchema(schemaVariant, ref dummyDictionary);

                // Act & Assert
                Assert.That(() => schema.ExtractVariables(jsonVariant), Throws.TypeOf<JsonSchemaValidationException>());
            }
        }

        [TestCaseSource(nameof(SchemasFromResources))]
        public void ShouldNotPassWhenUnexpectedPropertiesAreIncluded(string jsonSchema)
        {
            // Arrange
            var schema = JsonSchema.Parse(jsonSchema);

            foreach (var schemaVariant in EnumerateSchemaVariantsWithUnexpectedProperties(jsonSchema))
            {
                var dummyDictionary = new Dictionary<string, dynamic>();
                var jsonVariant = CreateJsonForSchema(schemaVariant, ref dummyDictionary);

                // Act & Assert
                Assert.That(() => schema.ExtractVariables(jsonVariant), Throws.TypeOf<JsonSchemaValidationException>());
            }
        }

        #region Utility methods for generating JSONs

        /// <summary>
        /// For a set O of optional properties included in the specified JSON schema,
        /// this method enumerates through its power set P(O). Thus, each possible combination
        /// of included/excluded optional property will be yielded.
        /// </summary>
        private static IEnumerable<string> EnumerateSchemaVariantsWithAllSubsetsOfOptionalProperties(string jsonSchema)
        {
            var schema = JObject.Parse(jsonSchema);
            var options = GetProperties(schema, "~").Length;

            foreach (var combination in options.GetBinaryCombinations())
            {
                var variant = JObject.Parse(jsonSchema);
                var propertiesToRemove = GetProperties(variant, "~")
                    .Where((p, i) => combination[i]);

                foreach (var property in propertiesToRemove)
                    property.Remove();

                yield return variant.ToString();
            }
        }

        /// <summary>
        /// For a set R of required properties included in the specified JSON schema,
        /// this method yields |R| variants - with one required property missing in each.
        /// |R| stands for the cardinality.
        /// </summary>
        private static IEnumerable<string> EnumerateSchemaVariantsWithMissingRequiredProperties(string jsonSchema)
        {
            var schema = JObject.Parse(jsonSchema);
            var options = GetProperties(schema, "~", false).Length;

            for (var i = 0; i < options; ++i)
            {
                var variant = JObject.Parse(jsonSchema);
                var propertyToRemove = GetProperties(variant, "~", false)[i];
                propertyToRemove.Remove();

                yield return variant.ToString();
            }
        }

        /// <summary>
        /// For a set J of JSON objects included in the specified schema, this method
        /// yields |J| variants - with one additional property included in each.
        /// </summary>
        private static IEnumerable<string> EnumerateSchemaVariantsWithUnexpectedProperties(string jsonSchema)
        {
            var schema = JObject.Parse(jsonSchema);
            var options = schema.Descendants<JObject>().Count();

            for (var i = 0; i < options; ++i)
            {
                var variant = JObject.Parse(jsonSchema);
                var objectToAddProperty = variant.Descendants<JObject>().ToArray()[i];
                objectToAddProperty.Add("additional property", "some value");

                yield return variant.ToString();
            }
        }

        private static JProperty[] GetProperties(JObject obj, string expressionMark, bool contains = true)
        {
            return obj.Descendants<JProperty>(p => p.Name.Contains(expressionMark) == contains)
                .ToArray();
        }

        /// <summary>
        /// Returns a JSON with schema expressions replaced with actual values.
        /// Also, fills a dictionary of variables expected to be extracted from the resulting JSON.
        /// </summary>
        private static string CreateJsonForSchema(string jsonSchema, ref Dictionary<string, dynamic> variables)
        {
            var schema = JObject.Parse(jsonSchema);

            foreach (var value in schema.ValuesByType(JTokenType.String))
            {
                var valueText = (string) value.Value;
                if (valueText.Contains("STRING"))
                {
                    value.Value = valueText.Replace("STRING @text", "someText");
                    variables.Add(valueText.Replace("STRING @", ""), value.Value);
                }
                if (valueText.Contains("JOBJECT"))
                {
                    var parentProperty = (JProperty) value.Parent;
                    parentProperty.Value = new JObject();
                    variables.Add(valueText.Replace("JOBJECT @", ""), parentProperty.Value);
                }
            }

            foreach (var property in GetProperties(schema, "@"))
            {
                var newName = property.Name.Replace("~", "").Replace("@", "");
                variables.Add(newName, newName);
            }

            return schema.ToString().Replace("~", "").Replace("@", ""); // hack to rename properties
        }

        #endregion

        #endregion
    }
}
