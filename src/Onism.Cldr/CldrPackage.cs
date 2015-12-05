using System.IO;
using System.IO.Compression;
using System.Net;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents one of the packages the CLDR data has been grouped into. This is a "smart enum" type.
    /// </summary>
    public abstract class CldrPackage
    {
        /// <summary>
        /// Gets or sets the name of this package.
        /// </summary>
        public string Name { get; protected set; }

        protected CldrPackage(string name)
        {
            Name = $"cldr-{name}";
        }

        /// <summary>
        /// Downloads this CLDR package from GitHub to a local directory.
        /// </summary>
        /// <param name="destinationDirectoryName">The path to the directory in which to place the extracted files.</param>
        public void Download(string destinationDirectoryName)
        {
            using (var client = new WebClient())
            {
                var zipPath = Path.Combine(destinationDirectoryName, $"{Name}.zip");
                var extractPath = Path.Combine(destinationDirectoryName, $"{Name}");
                var uri = $"https://github.com/unicode-cldr/{Name}/archive/master.zip";

                client.DownloadFile(uri, zipPath);
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(zipPath);
            }
        }

        /// <summary>
        /// Gets all available packages.
        /// </summary>
        public static CldrPackage[] GetPackages => new CldrPackage[]
        {
            Core,
            CalendarBuddhist,
            CalendarChinese,
            CalendarCoptic,
            CalendarDangi,
            CalendarEthiopic,
            CalendarHebrew,
            CalendarIndian,
            CalendarIslamic,
            CalendarJapanese,
            CalendarPersian,
            CalendarRoc,
            Dates,
            LocaleNames,
            Miscellaneous,
            Numbers,
            Segments,
            Units
        };

        /// <summary>
        /// Basic CLDR supplemental data.
        /// </summary>
        public static CldrSupplementalPackage Core => new CldrSupplementalPackage("core");

        /// <summary>
        /// Data for the Buddhist calendar.
        /// </summary>
        public static CldrStandardPackage CalendarBuddhist => new CldrStandardPackage("cal-buddhist");

        /// <summary>
        /// Data for the Chinese calendar.
        /// </summary>
        public static CldrStandardPackage CalendarChinese => new CldrStandardPackage("cal-chinese");
        
        /// <summary>
        /// Data for the Coptic calendar.
        /// </summary>
        public static CldrStandardPackage CalendarCoptic => new CldrStandardPackage("cal-coptic");
        
        /// <summary>
        /// Data for the Dangi calendar.
        /// </summary>
        public static CldrStandardPackage CalendarDangi => new CldrStandardPackage("cal-dangi");

        /// <summary>
        /// Data for the Ethiopic calendar.
        /// </summary>
        public static CldrStandardPackage CalendarEthiopic => new CldrStandardPackage("cal-ethiopic");
        
        /// <summary>
        /// Data for the Hebrew calendar.
        /// </summary>
        public static CldrStandardPackage CalendarHebrew => new CldrStandardPackage("cal-hebrew");
        
        /// <summary>
        /// Data for the Indian calendar.
        /// </summary>
        public static CldrStandardPackage CalendarIndian => new CldrStandardPackage("cal-indian");
        
        /// <summary>
        /// Data for the Islamic calendar.
        /// </summary>
        public static CldrStandardPackage CalendarIslamic => new CldrStandardPackage("cal-islamic");
        
        /// <summary>
        /// Data for the Japanese calendar.
        /// </summary>
        public static CldrStandardPackage CalendarJapanese => new CldrStandardPackage("cal-japanese");
        
        /// <summary>
        /// Data for the Persian calendar.
        /// </summary>
        public static CldrStandardPackage CalendarPersian => new CldrStandardPackage("cal-persian");
        
        /// <summary>
        /// Data for the Republic of China calendar.
        /// </summary>
        public static CldrStandardPackage CalendarRoc => new CldrStandardPackage("cal-roc");

        /// <summary>
        /// Data for date/time formatting, including data for the Gregorian calendar.
        /// </summary>
        public static CldrStandardPackage Dates => new CldrStandardPackage("dates");

        /// <summary>
        /// Translated versions of locale display name elements: languages, scripts, territories, and variants.
        /// </summary>
        public static CldrStandardPackage LocaleNames => new CldrStandardPackage("localenames");
        
        /// <summary>
        /// Other CLDR data not defined elsewhere.
        /// </summary>
        public static CldrStandardPackage Miscellaneous => new CldrStandardPackage("misc");
        
        /// <summary>
        /// Data for number formatting.
        /// </summary>
        public static CldrStandardPackage Numbers => new CldrStandardPackage("numbers");
        
        /// <summary>
        /// Line breaking data from Unicode's ULI project.
        /// </summary>
        public static CldrSegmentsPackage Segments => new CldrSegmentsPackage("segments-modern");

        /// <summary>
        /// Data for units formatting.
        /// </summary>
        public static CldrStandardPackage Units => new CldrStandardPackage("units");
    }
}
