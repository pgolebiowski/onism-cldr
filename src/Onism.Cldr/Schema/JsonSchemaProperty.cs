using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Schema
{
    public class JsonSchemaProperty
    {
        public string Name { get; }
        public bool IsRequired { get; }
        public bool IsVariable { get; }

        private JsonSchemaProperty(string name, bool isRequired, bool isVariable)
        {
            this.Name = name;
            this.IsRequired = isRequired;
            this.IsVariable = isVariable;
        }

        public static JsonSchemaProperty Parse(string expression)
        {
            var pattern = @"^(?<tilde>~?)(?<at>@?)(?<name>.+)$";
            var match = Regex.Match(expression, pattern);

            if (!match.Success)
                throw new ArgumentException($"'{expression}' is not a valid expression.");

            return new JsonSchemaProperty
            (
                name: match.GetMatchedGroup("name"),
                isRequired: !match.IsGroupMatched("tilde"),
                isVariable: match.IsGroupMatched("at")
            );
        }
    }
}
