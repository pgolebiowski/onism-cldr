using System;
using System.Collections.Generic;

namespace Onism.Cldr.Test.Utils
{
    internal static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();
            Shuffle(list, () => random.Next(list.Count + 1));
        }

        public static void Shuffle<T>(this IList<T> list, DeterministicRandom random)
        {
            var elementsCount = list.Count;
            Shuffle(list, () => random.NextInt() % elementsCount);
        }

        public static void Shuffle<T>(this IList<T> list, Func<int> randomIndexProvider)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = randomIndexProvider();
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
