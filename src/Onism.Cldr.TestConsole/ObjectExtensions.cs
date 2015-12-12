using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onism.Cldr.TestConsole
{
    public static class ObjectExtensions
    {
        public static T Print<T>(this T source)
        {
            Console.WriteLine(source);
            return source;
        }
    }
}
