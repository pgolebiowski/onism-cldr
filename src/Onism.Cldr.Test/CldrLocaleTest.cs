using System;
using NUnit.Framework;
using FluentAssertions;

namespace Onism.Cldr.Test
{
    [TestFixture]
    public class CldrLocaleTest
    {
        private static CldrLocale Arrange(string language, string territory, string script, string variant)
        {
            return new CldrLocale(
                language: language,
                territory: territory,
                script: script,
                variant: variant);
        }

        private static CldrLocale EnGb => Arrange("en", "GB", null, null);

        [Test]
        public void Equals_InstanceAndNull_False()
        {
            var l = EnGb;

            Equals(l, null).Should().BeFalse();
            Equals(null, l).Should().BeFalse();

            (l == null).Should().BeFalse();
            (null == l).Should().BeFalse();
        }

        [Test]
        public void Equals_SameValues_True()
        {
            var l1 = EnGb;
            var l2 = EnGb;

            l1.Should().Be(l2);
            l2.Should().Be(l1);

            (l1 == l2).Should().BeTrue();
            (l2 == l1).Should().BeTrue();
        }

        [Test]
        public void Equals_SameReferences_True()
        {
            var l1 = EnGb;
            var l2 = l1;

            l1.Should().Be(l2);
            l2.Should().Be(l1);

            (l1 == l2).Should().BeTrue();
            (l2 == l1).Should().BeTrue();

            Action a = () => EnGb.GetHashCode();
            a.ShouldNotThrow();
        }

        [Test]
        public void Equals_DifferentTypes_False()
        {
            object l1 = EnGb;
            object l2 = "EnGb";

            l1.Should().NotBe(l2);
            l2.Should().NotBe(l1);

            (l1 == l2).Should().BeFalse();
            (l2 == l1).Should().BeFalse();
        }

        [Test]
        public void Equals_SameButBoxed_True()
        {
            object l1 = EnGb;
            object l2 = EnGb;

            l1.Should().Be(l2);
            l2.Should().Be(l1);
        }

        [Test]
        public void Equals_SlightlyDifferent_False()
        {
            var l = new CldrLocale("a", "b", "c", "d");

            l.Should().NotBe(new CldrLocale("a", "b", "c", "e"));
            l.Should().NotBe(new CldrLocale("a", "b", "d", "d"));
            l.Should().NotBe(new CldrLocale("a", "d", "c", "d"));
            l.Should().NotBe(new CldrLocale("d", "b", "c", "d"));
            l.Should().Be(new CldrLocale("a", "b", "c", "d"));
        }
    }
}
