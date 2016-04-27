using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using Onism.Cldr.Schema;
using Onism.Cldr.Test.Schema.Resources;
using Onism.Cldr.Test.Schema.Resources.Locales;

namespace Onism.Cldr.Test.Schema
{
    [TestFixture]
    public class CldrLocaleFilterTest
    {
        private static readonly string[] FullLocaleList = Regex.Split(LocaleList.Full, "\r\n|\r|\n");

        [Test]
        public void WhenIncludeAndExcludeAreEmpty()
        {
            // Arrange
            var include = Array.Empty<string>();
            var exclude = Array.Empty<string>();
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            CollectionAssert.AreEqual(FullLocaleList, allowed);
        }

        [Test]
        public void WhenIncludeIsEmptyAndExcludeContainsSpecificLocales()
        {
            // Arrange
            var include = Array.Empty<string>();
            var exclude = new[] { "en-US", "en-GB" };
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            CollectionAssert.AreEquivalent(FullLocaleList.Except(exclude), allowed);
        }

        [Test]
        public void WhenIncludeIsEmptyAndExcludeContainsWildcardLocales()
        {
            // Arrange
            var include = Array.Empty<string>();
            var exclude = new[] { "en-*", "fr-*" };
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            var expectedSet = FullLocaleList.Where(x => !(x.StartsWith("en-") || x.StartsWith("fr-")));
            CollectionAssert.AreEquivalent(expectedSet, allowed);
        }

        [Test]
        public void WhenExcludeIsEmptyAndincludeContainsSpecificLocales()
        {
            // Arrange
            var include = new[] { "en", "en-GB" };
            var exclude = Array.Empty<string>();
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            CollectionAssert.AreEquivalent(include, allowed);
        }

        [Test]
        public void WhenExcludeIsEmptyAndIncludeContainsWildcardLocales()
        {
            // Arrange
            var include = new[] { "en-*", "fr-*" };
            var exclude = Array.Empty<string>();
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            var expectedSet = FullLocaleList.Where(x => (x.StartsWith("en-") || x.StartsWith("fr-")));
            CollectionAssert.AreEquivalent(expectedSet, allowed);
        }

        [Test]
        public void WhenBothIncludeAndExcludeContainSpecificLocales()
        {
            // Arrange
            var include = new[] { "en", "en-GB", "en-NL" };
            var exclude = new[] { "en-NL" };
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "en", "en-GB" }, allowed);
        }

        [Test]
        public void WhenBothIncludeAndExcludeContainWildcards()
        {
            // Arrange
            var include = new[] { "zh*" };
            var exclude = new[] { "zh-Hans-*", "zh-Hant-*" };
            var filter = new CldrLocaleFilter(include, exclude);

            // Act
            var allowed = FullLocaleList.Where(filter.IsAllowed);

            // Assert
            var expectedSet = new[] { "zh", "zh-Hans", "zh-Hant" };
            CollectionAssert.AreEquivalent(expectedSet, allowed);
        }
    }
}
