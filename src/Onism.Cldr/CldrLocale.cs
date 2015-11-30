using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Onism.Cldr
{
    /// <summary>
    /// Unicode locale identifier used in CLDR JSON data.
    /// Consists of language, territory, script, and variant subtags.
    /// </summary>
    /// <remarks>
    /// The locale identifier used in CLDR JSON data is in fact a language identifier,
    /// since no locale extensions are being used. However, this might be changed in future CLDR releases.
    /// See the Core Specification (http://cldr.unicode.org/core-spec) for definitions.
    /// </remarks>
    public class CldrLocale
    {
        /// <summary>
        /// Unicode language subtag (also known as a Unicode base language code).
        /// </summary>
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        /// <summary>
        /// Unicode region subtag (also known as a Unicode region code, or a Unicode territory code).
        /// </summary>
        [JsonProperty(PropertyName = "territory")]
        public string Territory { get; set; }

        /// <summary>
        /// Unicode script subtag (also known as a Unicode script code).
        /// </summary>
        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }

        /// <summary>
        /// Unicode variant subtag (also known as a Unicode language variant code)
        /// </summary>
        [JsonProperty(PropertyName = "variant")]
        public string Variant { get; set; }

        /// <summary>
        /// A locale that only has a language subtag (and optionally a script subtag)
        /// is called a language locale.
        /// </summary>
        public bool IsLanguageLocale => (Language != null) && (Territory == null) && (Variant == null);

        /// <summary>
        /// A locale with both language and territory subtag is called
        /// a territory locale (or country locale).
        /// </summary>
        public bool IsTerritoryLocale => (Language != null) && (Territory != null);

        /// <summary>
        /// Gets the hyphen-separated code of this <see cref="CldrLocale"/>.
        /// </summary>
        public string Code => JoinNotNull("-", Language, Territory, Script, Variant);

        private string JoinNotNull(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(x => x != null));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(CldrLocale))
                return false;

            return Equals((CldrLocale)obj);
        }

        public bool Equals(CldrLocale other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Language == other.Language
                && Territory == other.Territory
                && Script == other.Script
                && Variant == other.Variant;
        }

        public override int GetHashCode()
        {
            return (new string[] { Language, Territory, Script, Variant })
                .Select(x => x == null ? 0 : x.GetHashCode())
                .Aggregate((a, b) => a ^ b);
        }

        public static bool operator ==(CldrLocale left, CldrLocale right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CldrLocale left, CldrLocale right)
        {
            return !Equals(left, right);
        }
    }
}
