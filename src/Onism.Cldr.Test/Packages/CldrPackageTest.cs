using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Onism.Cldr.Packages;

namespace Onism.Cldr.Test.Packages
{
    [TestFixture]
    public class CldrPackageTest
    {
        [Test]
        public void GetPackages_ShouldReturnAllStaticInstances()
        {
            var staticInstances = typeof(CldrPackage)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(x => x.GetValue(null))
                .Where(x => x is CldrPackage)
                .Cast<CldrPackage>()
                .ToArray();

            var a = staticInstances;
            var b = CldrPackage.GetPackages();

            Func<object, object, bool> sameType = (x1, x2) => x1.GetType() == x2.GetType();
            Func<CldrPackage, CldrPackage, bool> sameName = (x1, x2) => x1.Name == x2.Name;

            a.All(x => b.Any(y => sameType(x, y) && sameName(x, y))).Should().BeTrue();
            b.All(x => a.Any(y => sameType(x, y) && sameName(x, y))).Should().BeTrue();
        }
    }
}
