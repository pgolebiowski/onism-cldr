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
        /// Validates a file extracted from this package and creates a temporary representation
        /// of the data to be later consumed while building a <see cref="CldrTree"/>.
        /// </summary>
        internal override CldrJson TryParseFile(string path)
        {
            var localeCode = Path.GetFileName(Path.GetDirectoryName(path));

            // root
            var o = JsonUtils.LoadFromFile(path)
                .DescendantsTypesShouldOnlyBe(JTokenType.Object, JTokenType.Property, JTokenType.String)
                .PropertiesCountShouldBe(1)
                .PropertiesShouldContain("main", JTokenType.Object);

            // main
            var main = ((JObject) o["main"])
                .PropertiesCountShouldBe(1)
                .PropertiesShouldContain(localeCode, JTokenType.Object);

            // en-GB
            var locale = ((JObject) main[localeCode])
                .PropertiesCountShouldBe(2)
                .PropertiesShouldContain("identity", JTokenType.Object);

            // extract the information
            var identity = locale["identity"]
                .ToObject<CldrLocale>()
                .LocaleCodeShouldBe(localeCode);

            var someProperty = locale
                .Properties()
                .First(x => x.Name != "identity");

            var someData = new JObject(someProperty);

            return new CldrJson(GetType(), identity, someData);
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