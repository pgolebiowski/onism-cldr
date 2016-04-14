using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Onism.Cldr.Schema;

namespace Onism.Cldr.Test.Schema
{
    [TestFixture]
    public class JsonSchemaPropertyTest
    {
        [TestCase("text", Result = "text")]
        [TestCase("~text", Result = "text")]
        [TestCase("@text", Result = "text")]
        [TestCase("~@text", Result = "text")]
        [TestCase("~@爱", Result = "爱")]
        public string NameShouldBeExtracted(string expression)
        {
            return JsonSchemaProperty.Parse(expression).Name;
        }

        [TestCase("text", Result = false)]
        [TestCase("~text", Result = true)]
        [TestCase("@text", Result = false)]
        [TestCase("~@text", Result = true)]
        [TestCase("~@爱", Result = true)]
        public bool OptionalityShouldBeExtracted(string expression)
        {
            return JsonSchemaProperty.Parse(expression).IsOptional;
        }

        [TestCase("text", Result = false)]
        [TestCase("~text", Result = false)]
        [TestCase("@text", Result = true)]
        [TestCase("~@text", Result = true)]
        [TestCase("~@爱", Result = true)]
        public bool VariablesShouldBeDetected(string expression)
        {
            var result = JsonSchemaProperty.Parse(expression).IsVariable;
            return result;
        }
    }
}
