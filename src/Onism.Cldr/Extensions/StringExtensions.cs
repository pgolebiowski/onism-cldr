using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Onism.Cldr.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Removes diacritics from each glyph in the specified text.
        /// </summary>
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

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
