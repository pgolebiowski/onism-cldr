using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class IdentifierDictionaryTest
    {
        [Test]
        public void NewItemsShouldBeAssignedIdentifiersInSequentialOrder()
        {
            // Arrange
            var dictionary = new IdentifierDictionary<string>();

            // Act
            var a = dictionary.GetId("a");
            var b = dictionary.GetId("b");
            var c = dictionary.GetId("c");

            // Assert
            Assert.That(dictionary.Count, Is.EqualTo(3));
            Assert.That(a, Is.EqualTo(0));
            Assert.That(b, Is.EqualTo(1));
            Assert.That(c, Is.EqualTo(2));
        }

        [Test]
        public void ItemsShouldNotBeAssignedNewIdentifiersWhenCheckedAgain()
        {
            // Arrange
            var dictionary = new IdentifierDictionary<string>();

            // Act
            for (var i = 0; i < 30; ++i)
                dictionary.GetId((i % 3).ToString());

            // Assert
            Assert.That(dictionary.Count, Is.EqualTo(3));
            Assert.That(dictionary.GetId("0"), Is.EqualTo(0));
            Assert.That(dictionary.GetId("1"), Is.EqualTo(1));
            Assert.That(dictionary.GetId("2"), Is.EqualTo(2));
        }

        [Test]
        public void ItemsShouldBeRetrieved()
        {
            // Arrange
            var dictionary = new IdentifierDictionary<string>();
            var items = new[] {"a", "b", "c"};
            
            // Act
            foreach (var item in items)
                dictionary.GetId(item);
            
            // Assert
            CollectionAssert.AreEqual(items, dictionary.Items);
        }

        [Test]
        public void SameDictionariesShouldBeEqual()
        {
            // Arrange
            var d1 = new IdentifierDictionary<string>();
            var d2 = new IdentifierDictionary<string>();
            var items = new[] {"a", "b", "c", "d", "e"};

            // Act
            foreach (var item in items)
            {
                d1.GetId(item);
                d2.GetId(item);
            }

            // Assert
            Assert.AreEqual(d1, d2);
        }

        [Test]
        public void DictionariesWithSameElementsButDifferentOrderOfAdditionShouldNotBeEqual()
        {
            // Arrange
            var d1 = new IdentifierDictionary<string>();
            var d2 = new IdentifierDictionary<string>();
            var items1 = new[] { "a", "b", "c", "d", "e" };
            var items2 = new[] { "a", "b", "d", "c", "e" };

            // Act
            foreach (var item in items1)
                d1.GetId(item);

            foreach (var item in items2)
                d2.GetId(item);

            // Assert
            Assert.AreNotEqual(d1, d2);
        }
    }
}
