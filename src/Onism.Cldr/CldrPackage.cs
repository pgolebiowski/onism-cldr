using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents one of the packages the CLDR data has been grouped into. This is a "smart enum" type.
    /// </summary>
    /// <remarks>
    /// Because the CLDR is so large and contains so many different types of information, the JSON data
    /// is grouped into packages by functionality. For each type of functionality, there are two available
    /// packages: The "modern" packages, which contain the set of locales listed as modern coverage targets
    /// by the CLDR subcomittee, and the "full" packages, which contain the complete set of locales,
    /// including those in the corresponding modern packages.
    /// 
    /// For more information visit: https://github.com/unicode-cldr/cldr-json.
    /// </remarks>
    public sealed class CldrPackage
    {
        private string name;

        private CldrPackage(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Downloads this CLDR package from GitHub to a local directory.
        /// </summary>
        /// <param name="destinationDirectoryName">The path to the directory in which to place the extracted files.</param>
        public void Download(string destinationDirectoryName)
        {
            using (var client = new WebClient())
            {
                var zipPath = Path.Combine(destinationDirectoryName, $"{name}.zip");
                var extractPath = Path.Combine(destinationDirectoryName, $"{name}");
                var uri = $"https://github.com/unicode-cldr/cldr-{name}/archive/master.zip";

                client.DownloadFile(uri, zipPath);
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(zipPath);
            }
        }

        /// <summary>
        /// Returns all the available packages.
        /// </summary>
        public static CldrPackage[] GetPackages() => new CldrPackage[]
        {
            CALENDAR_BUDDHIST_FULL,
            CALENDAR_BUDDHIST_MODERN,
            CALENDAR_CHINESE_FULL,
            CALENDAR_CHINESE_MODERN,
            CALENDAR_COPTIC_FULL,
            CALENDAR_COPTIC_MODERN,
            CALENDAR_DANGI_FULL,
            CALENDAR_DANGI_MODERN,
            CALENDAR_ETHIOPIC_FULL,
            CALENDAR_ETHIOPIC_MODERN,
            CALENDAR_HEBREW_FULL,
            CALENDAR_HEBREW_MODERN,
            CALENDAR_INDIAN_FULL,
            CALENDAR_INDIAN_MODERN,
            CALENDAR_ISLAMIC_FULL,
            CALENDAR_ISLAMIC_MODERN,
            CALENDAR_JAPANESE_FULL,
            CALENDAR_JAPANESE_MODERN,
            CALENDAR_PERSIAN_FULL,
            CALENDAR_PERSIAN_MODERN,
            CALENDAR_ROC_FULL,
            CALENDAR_ROC_MODERN,
            CORE,
            DATES_FULL,
            DATES_MODERN,
            LOCALENAMES_FULL,
            LOCALENAMES_MODERN,
            MISCELLANEOUS_FULL,
            MISCELLANEOUS_MODERN,
            NUMBERS_FULL,
            NUMBERS_MODERN,
            SEGMENTS_MODERN,
            UNITS_FULL,
            UNITS_MODERN
        };

        /// <summary>
        /// Basic CLDR supplemental data.
        /// </summary>
        public static readonly CldrPackage CORE = new CldrPackage("core");

        /// <summary>
        /// Data for date/time formatting, including Data for the Gregorian calendar.
        /// </summary>
        public static readonly CldrPackage DATES_FULL = new CldrPackage("dates-full");

        /// <summary>
        /// Data for date/time formatting, including Data for the Gregorian calendar.
        /// </summary>
        public static readonly CldrPackage DATES_MODERN = new CldrPackage("dates-modern");

        /// <summary>
        /// Data for the Buddhist calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_BUDDHIST_FULL = new CldrPackage("cal-buddhist-full");

        /// <summary>
        /// Data for the Buddhist calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_BUDDHIST_MODERN = new CldrPackage("cal-buddhist-modern");

        /// <summary>
        /// Data for the Chinese calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_CHINESE_FULL = new CldrPackage("cal-chinese-full");

        /// <summary>
        /// Data for the Chinese calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_CHINESE_MODERN = new CldrPackage("cal-chinese-modern");

        /// <summary>
        /// Data for the Coptic calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_COPTIC_FULL = new CldrPackage("cal-coptic-full");

        /// <summary>
        /// Data for the Coptic calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_COPTIC_MODERN = new CldrPackage("cal-coptic-modern");

        /// <summary>
        /// Data for the Dangi calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_DANGI_FULL = new CldrPackage("cal-dangi-full");

        /// <summary>
        /// Data for the Dangi calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_DANGI_MODERN = new CldrPackage("cal-dangi-modern");

        /// <summary>
        /// Data for the Ethiopic calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_ETHIOPIC_FULL = new CldrPackage("cal-ethiopic-full");

        /// <summary>
        /// Data for the Ethiopic calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_ETHIOPIC_MODERN = new CldrPackage("cal-ethiopic-modern");

        /// <summary>
        /// Data for the Hebrew calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_HEBREW_FULL = new CldrPackage("cal-hebrew-full");

        /// <summary>
        /// Data for the Hebrew calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_HEBREW_MODERN = new CldrPackage("cal-hebrew-modern");

        /// <summary>
        /// Data for the Indian calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_INDIAN_FULL = new CldrPackage("cal-indian-full");

        /// <summary>
        /// Data for the Indian calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_INDIAN_MODERN = new CldrPackage("cal-indian-modern");

        /// <summary>
        /// Data for the Islamic calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_ISLAMIC_FULL = new CldrPackage("cal-islamic-full");

        /// <summary>
        /// Data for the Islamic calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_ISLAMIC_MODERN = new CldrPackage("cal-islamic-modern");

        /// <summary>
        /// Data for the Japanese calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_JAPANESE_FULL = new CldrPackage("cal-japanese-full");

        /// <summary>
        /// Data for the Japanese calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_JAPANESE_MODERN = new CldrPackage("cal-japanese-modern");

        /// <summary>
        /// Data for the Persian calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_PERSIAN_FULL = new CldrPackage("cal-persian-full");

        /// <summary>
        /// Data for the Persian calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_PERSIAN_MODERN = new CldrPackage("cal-persian-modern");

        /// <summary>
        /// Data for the Republic of China calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_ROC_FULL = new CldrPackage("cal-roc-full");

        /// <summary>
        /// Data for the Republic of China calendar.
        /// </summary>
        public static readonly CldrPackage CALENDAR_ROC_MODERN = new CldrPackage("cal-roc-modern");

        /// <summary>
        /// Translated versions of locale display name elements: languages, scripts, territories, and variants.
        /// </summary>
        public static readonly CldrPackage LOCALENAMES_FULL = new CldrPackage("localenames-full");

        /// <summary>
        /// Translated versions of locale display name elements: languages, scripts, territories, and variants.
        /// </summary>
        public static readonly CldrPackage LOCALENAMES_MODERN = new CldrPackage("localenames-modern");

        /// <summary>
        /// Other CLDR data not defined elsewhere.
        /// </summary>
        public static readonly CldrPackage MISCELLANEOUS_FULL = new CldrPackage("misc-full");

        /// <summary>
        /// Other CLDR data not defined elsewhere.
        /// </summary>
        public static readonly CldrPackage MISCELLANEOUS_MODERN = new CldrPackage("misc-modern");

        /// <summary>
        /// Data for number formatting.
        /// </summary>
        public static readonly CldrPackage NUMBERS_FULL = new CldrPackage("numbers-full");

        /// <summary>
        /// Data for number formatting.
        /// </summary>
        public static readonly CldrPackage NUMBERS_MODERN = new CldrPackage("numbers-modern");

        /// <summary>
        /// Line breaking data from Unicode's ULI project.
        /// </summary>
        public static readonly CldrPackage SEGMENTS_MODERN = new CldrPackage("segments-modern");

        /// <summary>
        /// Data for units formatting.
        /// </summary>
        public static readonly CldrPackage UNITS_FULL = new CldrPackage("units-full");

        /// <summary>
        /// Data for units formatting.
        /// </summary>
        public static readonly CldrPackage UNITS_MODERN = new CldrPackage("units-modern");
    }
}
