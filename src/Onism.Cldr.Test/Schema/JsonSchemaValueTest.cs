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
        [TestCase("type @text", Result = "type")]
        [TestCase("爱 @text", Result = "爱")]
        public string TypeShouldBeExtracted(string expression)
        {
            return JsonSchemaValue.Parse(expression).Type;
        }

        [TestCase("type text", Result = "text")]
        [TestCase("type @text", Result = "text")]
        [TestCase("type @爱", Result = "爱")]
        public string NameShouldBeExtracted(string expression)
        {
            return JsonSchemaValue.Parse(expression).Name;
        }

        [TestCase("type text", Result = false)]
        [TestCase("type @text", Result = true)]
        [TestCase("type @爱", Result = true)]
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
