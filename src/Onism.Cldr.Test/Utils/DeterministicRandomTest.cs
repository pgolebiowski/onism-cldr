using System.Linq;
using NUnit.Framework;

namespace Onism.Cldr.Test.Utils
{
    [TestFixture]
    public class DeterministicRandomTest
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void BoolValuesShouldBeReturnedInCycle(int length)
        {
            // Arrange
            var random = new DeterministicRandom(length);

            // Act
            var cycle1 = Enumerable.Range(1, length).Select(i => random.NextBool()).ToArray();
            var cycle2 = Enumerable.Range(1, length).Select(i => random.NextBool()).ToArray();
            var cycle3 = Enumerable.Range(1, length).Select(i => random.NextBool()).ToArray();

            // Assert
            CollectionAssert.AreEqual(cycle1, cycle2);
            CollectionAssert.AreEqual(cycle2, cycle3);
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void IntegerValuesShouldBeReturnedInCycle(int length)
        {
            // Arrange
            var random = new DeterministicRandom(length);

            // Act
            var cycle1 = Enumerable.Range(1, length).Select(i => random.NextInt()).ToArray();
            var cycle2 = Enumerable.Range(1, length).Select(i => random.NextInt()).ToArray();
            var cycle3 = Enumerable.Range(1, length).Select(i => random.NextInt()).ToArray();

            // Assert
            CollectionAssert.AreEqual(cycle1, cycle2);
            CollectionAssert.AreEqual(cycle2, cycle3);
        }

        [TestCase("0")]
        [TestCase("1")]
        [TestCase("1000")]
        [TestCase("0000")]
        [TestCase("10010101010100101011111100000100110")]
        public void InitializedWithStringShouldWorkAsExpected(string bits)
        {
            // Arrange
            var length = bits.Length;
            var random = new DeterministicRandom(bits);

            // Act
            var cycle1 = Enumerable.Range(1, length).Select(i => random.NextBool()).ToArray();
            var cycle2 = Enumerable.Range(1, length).Select(i => random.NextBool()).ToArray();
            var cycle3 = Enumerable.Range(1, length).Select(i => random.NextBool()).ToArray();

            // Assert
            CollectionAssert.AreEqual(cycle1, cycle2);
            CollectionAssert.AreEqual(cycle2, cycle3);

            for (var i = 0; i < length; ++i)
            {
                var bit = bits[i] == '1';
                Assert.AreEqual(bit, cycle1[i]);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void InitializedRandomlyAndThenReinitializedWithString(int length)
        {
            // Arrange
            var random1 = new DeterministicRandom(length);
            var bits = random1.ToString();
            var random2 = new DeterministicRandom(bits);

            // Act
            var cycle11 = Enumerable.Range(1, length).Select(i => random1.NextBool()).ToArray();
            var cycle12 = Enumerable.Range(1, length).Select(i => random1.NextBool()).ToArray();
            var cycle13 = Enumerable.Range(1, length).Select(i => random1.NextBool()).ToArray();

            var cycle21 = Enumerable.Range(1, length).Select(i => random2.NextBool()).ToArray();
            var cycle22 = Enumerable.Range(1, length).Select(i => random2.NextBool()).ToArray();
            var cycle23 = Enumerable.Range(1, length).Select(i => random2.NextBool()).ToArray();

            // Assert
            CollectionAssert.AreEqual(cycle11, cycle12);
            CollectionAssert.AreEqual(cycle12, cycle13);

            CollectionAssert.AreEqual(cycle11, cycle21);
            CollectionAssert.AreEqual(cycle12, cycle22);
            CollectionAssert.AreEqual(cycle13, cycle23);
        }
    }
}