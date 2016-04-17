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
    public class JsonSchemaValueTest
    {
        [TestCase("type @text", ExpectedResult = "type")]
        [TestCase("爱 @text", ExpectedResult = "爱")]
        public string TypeShouldBeExtracted(string expression)
        {
            return JsonSchemaValue.Parse(expression).Type;
        }

        [TestCase("type text", ExpectedResult = "text")]
        [TestCase("type @text", ExpectedResult = "text")]
        [TestCase("type @爱", ExpectedResult = "爱")]
        public string NameShouldBeExtracted(string expression)
        {
            return JsonSchemaValue.Parse(expression).Name;
        }

        [TestCase("type text", ExpectedResult = false)]
        [TestCase("type @text", ExpectedResult = true)]
        [TestCase("type @爱", ExpectedResult = true)]
        public bool VariablesShouldBeDetected(string expression)
        {
            return JsonSchemaValue.Parse(expression).IsVariable;
        }

        [TestCase("text")]
        public void ShouldThrowExceptionForMalformedExpressions(string expression)
        {
            Assert.Throws<ArgumentException>(() => JsonSchemaValue.Parse(expression));
        }
    }
}
