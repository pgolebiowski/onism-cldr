using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Tools
{
    /// <summary>
    /// Represents a package containing localization data.
    /// </summary>
    public sealed class CldrStandardPackage : CldrPackage
    {
        internal CldrStandardPackage(string name)
            : base(name)
        {
            Name = $"{Name}-full";
        }

        /// <summary>
        /// Validates files discovered in this package and creates a temporary
        /// representation of the data to be later consumed while building a <see cref="CldrTree"/>.
        /// </summary>
        internal override void TryParsePackage(string directoryPath)
        {
            Data = (from path in CldrPackagePathExtractor.ExtractPaths(directoryPath)

                    let localeCode = Path.GetFileName(Path.GetDirectoryName(path))
                    let json = File.ReadAllText(path)

                    // root
                    let o = JObject.Parse(json)
                        .PropertiesCountShouldBe(1)
                        .PropertiesShouldContain("main", JTokenType.Object)

                    // main
                    let main = ((JObject)o["main"])
                        .PropertiesCountShouldBe(1)
                        .PropertiesShouldContain(localeCode, JTokenType.Object)

                    // en-GB
                    let locale = ((JObject)main[localeCode])
                        .PropertiesCountShouldBe(2)
                        .PropertiesShouldContain("identity", JTokenType.Object)

                    // extract the information
                    let identity = locale["identity"]
                        .ToObject<CldrLocale>()
                        .LocaleCodeShouldBe(localeCode)

                    let someProperty = locale
                        .Properties()
                        .First(x => x.Name != "identity")

                    let someData = new JObject(someProperty)

                    select new CldrJson(identity, someData))
                .ToArray();
        }

        /// <summary>
        /// Gets a flag indicating whether this package variant is "full".
        /// False means the variant is "modern". The default value is true.
        /// </summary>
        public bool IsFullVariant { get; private set; } = true;

        /// <summary>
        /// Sets this package to be of the "full" variant.
        /// </summary>
        public CldrStandardPackage AsFull()
        {
            if (IsFullVariant)
                return this;

            Name = Name.Replace("-modern", "-full");
            IsFullVariant = true;
            return this;
        }

        /// <summary>
        /// Sets this package to be of the "modern" variant.
        /// </summary>
        public CldrStandardPackage AsModern()
        {
            if (!IsFullVariant)
                return this;

            Name = Name.Replace("-full", "-modern");
            IsFullVariant = false;
            return this;
        }
    }
}