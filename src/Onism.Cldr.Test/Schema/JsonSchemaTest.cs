using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions.Equivalency;
using Newtonsoft.Json;
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
        [TestCase("{ }", Result = true)]
        [TestCase("{ 'a': '' }", Result = false)]
        public bool EmptySchema(string json)
        {
            return JsonSchema.Parse("{ }").Validate(json);
        }

        [TestCase("{ }", Result = false)]
        [TestCase("{ 'required': 'text' }", Result = true)]
        [TestCase("{ 'different': '' }", Result = false)]
        [TestCase("{ 'required': '', 'another': '' }", Result = false)]
        public bool KnownAndRequired(string json)
        {
            return JsonSchema.Parse("{ 'required': 'STRING whatever' }").Validate(json);
        }

        [TestCase("{ }", Result = true)]
        [TestCase("{ 'required': 'text' }", Result = true)]
        [TestCase("{ 'different': '' }", Result = false)]
        [TestCase("{ 'required': '', 'another': '' }", Result = false)]
        public bool KnownAndOptional(string json)
        {
            return JsonSchema.Parse("{ '~required': 'STRING whatever' }").Validate(json);
        }

        [TestCase("{ }", false)]
        [TestCase("{ 'property': '' }", true)]
        [TestCase("{ 'one': '', 'another': '' }", false)]
        public void UnknownAndRequired(string json, bool expectedResult)
        {
            var variables = JsonSchema.Parse("{ '@optional': 'STRING whatever' }").ExtractVariables(json);
            
            Assert.That(variables.Count.Equals(1));
            Assert.That(variables, Contains.Item(new KeyValuePair<string, string>("optional", "property")));
        }

        private static IEnumerable<string> SchemasFromResources => new[]
        {
            JsonResources.JsonSchema1
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
            throw new NotImplementedException();
        }

        [TestCaseSource(nameof(SchemasFromResources))]
        public void ShouldNotPassWhenUnexpectedPropertiesAreIncluded(string jsonSchema)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// For a set S of optional properties included in the specified JSON schema,
        /// this method enumerates through its power set P(S). Thus, each possible combination
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

        private static JProperty[] GetProperties(JObject obj, string expressionMark)
        {
            return obj.Descendants<JProperty>(p => p.Name.Contains(expressionMark))
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
    }
}
