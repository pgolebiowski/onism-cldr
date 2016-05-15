using System;
using NUnit.Framework;
using Onism.Cldr.Tools;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrVersionConsistencyAssurerTest
    {
        [Test]
        public void WhenNotUpdatedUnspecifiedShouldBeReturned()
        {
            // Arrange
            var assurer = new CldrVersionConsistencyAssurer();

            // Act & Assert
            Assert.That(assurer.Version, Is.EqualTo("unspecified"));
        }

        [Test]
        public void WhenUpdatedVersionShouldBeReturned()
        {
            // Arrange
            var assurer = new CldrVersionConsistencyAssurer();

            // Act
            assurer.AssureVersionIsConsistent("30", "dummy path");

            // Assert
            Assert.That(assurer.Version, Is.EqualTo("30"));
        }

        [Test]
        public void SequenceOfSameUpdatesShouldWork()
        {
            // Arrange
            var assurer = new CldrVersionConsistencyAssurer();

            // Act
            for (var i = 0; i < 100; ++i)
                assurer.AssureVersionIsConsistent("30", $"dummy path {i}");

            // Assert
            Assert.That(assurer.Version, Is.EqualTo("30"));
        }

        [Test]
        public void ShouldThrowExceptionWhenVersionsAreDifferent()
        {
            // Arrange
            var assurer = new CldrVersionConsistencyAssurer();

            // Act & Assert
            Assert.DoesNotThrow(() => assurer.AssureVersionIsConsistent("30", "dummy path 1"));
            Assert.Throws<FormatException>(() => assurer.AssureVersionIsConsistent("31", "dummy path 2"));
        }

        [Test]
        public void ShouldThrowExceptionWhenAfterManySameVersionsDifferentIsDetected()
        {
            // Arrange
            var assurer = new CldrVersionConsistencyAssurer();

            // Act & Assert
            for (var i = 0; i < 100; ++i)
                Assert.DoesNotThrow(() => assurer.AssureVersionIsConsistent("30", $"dummy path {i}"));

            Assert.Throws<FormatException>(() => assurer.AssureVersionIsConsistent("31", "dummy path 101"));
        }
    }
}
