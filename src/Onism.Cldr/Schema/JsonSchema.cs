using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Schema
{
    public class JsonSchema
    {
        private readonly JObject schema;

        private JsonSchema(JObject schema)
        {
            this.schema = schema;
        }

        public static JsonSchema Parse(string json)
        {
            var schema = JObject.Parse(json);
            
            // here some schema validation (for the schema itself) could do.
            // however, this system of schemas is purely internal.
            // thus, no particular need for the mess.

            return new JsonSchema(schema);
        }

        public bool Validate(string json)
        {
            try
            {
                ExtractVariables(json);
                return true;
            }
            catch (JsonSchemaValidationException)
            {
                return false;
            }
        }

        public bool TryExtractVariables(string json, out Dictionary<string, dynamic> variables)
        {
            try
            {
                variables = ExtractVariables(json);
                return true;
            }
            catch (JsonSchemaValidationException)
            {
                variables = null;
                return false;
            }
        }

        public Dictionary<string, dynamic> ExtractVariables(string json)
        {
            var variables = new Dictionary<string, dynamic>();
            var root = JObject.Parse(json);

            ProcessNode(schema, root, ref variables);
            return variables;
        }

        private static void ProcessNode(JObject schemaNode, JObject testNode, ref Dictionary<string, dynamic> variables)
        {
            foreach (var matching in MatchProperties(schemaNode, testNode, ref variables))
            {
                switch (matching.SchemaProperty.Value.Type)
                {
                    case JTokenType.Object:
                        var childSchemaNode = (JObject) matching.SchemaProperty.Value;
                        var childTestNodeType = matching.TestProperty.Value.Type;

                        if (childTestNodeType != JTokenType.Object)
                            throw new JsonSchemaValidationException(
                                $"Expected a token of type JObject, but found {childTestNodeType}.");

                        var childTestNode = (JObject) matching.TestProperty.Value;
                        ProcessNode(childSchemaNode, childTestNode, ref variables);
                        break;

                    case JTokenType.String:
                        var expression = JsonSchemaValue.Parse((string) matching.SchemaProperty.Value);
                        var expectedType = expression.Type;
                        var actualType = matching.TestProperty.Value.Type;

                        var areStrings = expectedType.Equals("STRING") && actualType.Equals(JTokenType.String);
                        var areObjects = expectedType.Equals("JOBJECT") && actualType.Equals(JTokenType.Object);

                        if (areStrings || areObjects)
                        {
                            if (expression.IsVariable)
                                variables.Add(expression.Name, matching.TestProperty.Value);
                        }
                        else
                            throw new JsonSchemaValidationException(
                               $"Expected {expectedType} type but found {actualType}.");

                        break;

                    default:
                        throw new JsonSchemaValidationException(
                            $"Unexpected type {matching.SchemaProperty.Value.Type}");
                }
            }
        }

        private class PropertyMatching
        {
            public JProperty SchemaProperty { get; }
            public JProperty TestProperty { get; }

            public PropertyMatching(JProperty schemaProperty, JProperty testProperty)
            {
                this.SchemaProperty = schemaProperty;
                this.TestProperty = testProperty;
            }
        }

        private static IEnumerable<PropertyMatching> MatchProperties(
            JObject schemaNode,
            JObject testNode,
            ref Dictionary<string, dynamic> variables)
        {
            var result = new List<PropertyMatching>();

            var testNodeProperties = testNode.Properties().ToDictionary(p => p.Name, p => p);
            var schemaNodeProperties = schemaNode.Properties().Select(p => new
            {
                Property = p,
                Expression = JsonSchemaProperty.Parse(p.Name)
            })
            .ToArray();

            var knownProperties = schemaNodeProperties.Where(p => !p.Expression.IsVariable);

            // current design assumes there is no more than one variable property.
            // probably it will be enough for validating CLDR JSONs.
            var unknownProperty = schemaNodeProperties.FirstOrDefault(p => p.Expression.IsVariable);

            foreach (var knownProperty in knownProperties)
            {
                var expectedName = knownProperty.Expression.Name;
                if (testNodeProperties.ContainsKey(expectedName))
                {
                    var matchedProperty = testNodeProperties[expectedName];
                    result.Add(new PropertyMatching(knownProperty.Property, matchedProperty));
                    testNodeProperties.Remove(expectedName);
                }
                else
                {
                    if (knownProperty.Expression.IsRequired)
                        throw new JsonSchemaValidationException($"Required property '{expectedName}' was not found.");
                }
            }

            if (unknownProperty != null)
            {
                if (testNodeProperties.Count > 1)
                    throw new JsonSchemaValidationException(
                        $"More than one possible match for {unknownProperty.Property.Name} " +
                        $"in a set [{testNodeProperties.Select(x => x.Key).JoinStrings()}].");

                if (testNodeProperties.Count == 1)
                {
                    var matchedProperty = testNodeProperties.First();
                    result.Add(new PropertyMatching(unknownProperty.Property, matchedProperty.Value));
                    variables.Add(unknownProperty.Expression.Name, matchedProperty.Key);
                    testNodeProperties.Remove(matchedProperty.Key);
                }
                else
                {
                    if (unknownProperty.Expression.IsRequired)
                        throw new JsonSchemaValidationException($"Required property '{unknownProperty.Property.Name}' was not found.");
                }
            }

            if (testNodeProperties.Count > 0)
                throw new JsonSchemaValidationException(
                    $"The following properties were not expected: [{testNodeProperties.Select(x => x.Key).JoinStrings()}]");

            return result;
        }
    }
}
