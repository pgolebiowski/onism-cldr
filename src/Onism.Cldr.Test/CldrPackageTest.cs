using System.Linq;
using System.Reflection;
using NUnit.Framework;
using FluentAssertions;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrPackageTest
    {
        /*
        [Test]
        public void Packages_ShouldContainExactlyTheirSet()
        {
            var staticInstances = typeof(CldrPackage)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(CldrPackage))
                .Select(f => (CldrPackage)f.GetValue(null));

            var a = staticInstances;
            var b = CldrPackage.GetPackages();

            a.All(x => b.Contains(x)).Should().BeTrue();
            b.All(x => a.Contains(x)).Should().BeTrue();
        }
        */
    }
}
