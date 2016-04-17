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
        [TestCase("text", ExpectedResult = "text")]
        [TestCase("~text", ExpectedResult = "text")]
        [TestCase("@text", ExpectedResult = "text")]
        [TestCase("~@text", ExpectedResult = "text")]
        [TestCase("~@爱", ExpectedResult = "爱")]
        public string NameShouldBeExtracted(string expression)
        {
            return JsonSchemaProperty.Parse(expression).Name;
        }

        [TestCase("text", ExpectedResult = true)]
        [TestCase("~text", ExpectedResult = false)]
        [TestCase("@text", ExpectedResult = true)]
        [TestCase("~@text", ExpectedResult = false)]
        [TestCase("~@爱", ExpectedResult = false)]
        public bool RequiredShouldBeDetected(string expression)
        {
            return JsonSchemaProperty.Parse(expression).IsRequired;
        }

        [TestCase("text", ExpectedResult = false)]
        [TestCase("~text", ExpectedResult = false)]
        [TestCase("@text", ExpectedResult = true)]
        [TestCase("~@text", ExpectedResult = true)]
        [TestCase("~@爱", ExpectedResult = true)]
        public bool VariablesShouldBeDetected(string expression)
        {
            var result = JsonSchemaProperty.Parse(expression).IsVariable;
            return result;
        }
    }
}
