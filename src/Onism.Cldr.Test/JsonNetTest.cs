using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Test
{
    /// <summary>
    /// Tests of the assumptions about Json.NET types.
    /// </summary>
    [TestFixture]
    public class JsonNetTest
    {
        [Test]
        public void DerivedFromJToken()
        {
            // Arrange
            var tokenType = typeof(JToken);
            var expected = new[]
            {
                typeof(JArray),
                typeof(JConstructor),
                typeof(JContainer),
                typeof(JObject),
                typeof(JProperty),
                typeof(JRaw),
                typeof(JToken),
                typeof(JValue)
            };

            // Act
            var actual = Assembly
                .GetAssembly(tokenType)
                .GetTypes()
                .Where(tokenType.IsAssignableFrom);

            // Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void DerivedFromJContainer()
        {
            // Arrange
            var tokenType = typeof(JContainer);
            var expected = new[]
            {
                typeof(JArray),
                typeof(JConstructor),
                typeof(JContainer),
                typeof(JObject),
                typeof(JProperty)
            };

            // Act
            var actual = Assembly
                .GetAssembly(tokenType)
                .GetTypes()
                .Where(tokenType.IsAssignableFrom);
            
            // Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
