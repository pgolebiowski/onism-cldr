using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onism.Cldr.Extensions
{
    public static class IntegerExtensions
    {
        public static bool[] ToBoolArray(this int n)
        {
            var bitArray = new BitArray(new[] { n });
            var boolArray = new bool[bitArray.Count];
            bitArray.CopyTo(boolArray, 0);

            return boolArray;
        }

        public static IEnumerable<bool[]> GetBinaryCombinations(this int n)
        {
            var max = 1 << n;
            for (var i = 0; i < max; ++i)
                yield return i.ToBoolArray();
        } 
    }
}
