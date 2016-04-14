using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Schema
{
    public class JsonSchemaValue
    {
        public string Type { get; }
        public string Name { get; }
        public bool IsVariable { get; }

        private JsonSchemaValue(string type, string name, bool isVariable)
        {
            this.Type = type;
            this.Name = name;
            this.IsVariable = isVariable;
        }

        public static JsonSchemaValue Parse(string expression)
        {
            var pattern = @"^(?<type>[^\\s]+) (?<at>@?)(?<name>.+)$";
            var match = Regex.Match(expression, pattern);

            if (!match.Success)
                throw new ArgumentException($"'{expression}' is not a valid expression.");

            return new JsonSchemaValue
            (
                type: match.GetMatchedGroup("type"),
                name: match.GetMatchedGroup("name"),
                isVariable: match.IsGroupMatched("at")
            );
        }
    }
}
