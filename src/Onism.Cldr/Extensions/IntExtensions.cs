using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onism.Cldr.Extensions
{
    public static class IntExtensions
    {
        public static BitArray ToBitArray(this int a)
        {
            return new BitArray(new[] { a });
        }
    }
}
