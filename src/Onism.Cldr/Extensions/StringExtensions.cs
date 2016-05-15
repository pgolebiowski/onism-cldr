using System.Collections.Generic;
using System.IO;

namespace Onism.Cldr.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Reads the specified text line by line.
        /// </summary>
        public static IEnumerable<string> EnumerateLines(this string text)
        {
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
