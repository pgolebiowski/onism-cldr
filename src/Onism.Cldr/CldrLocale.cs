using System.Linq;
using ProtoBuf;

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
    [ProtoContract]
    public sealed class CldrLocale
    {
        internal static readonly CldrLocale None = new CldrLocale("none");

        [ProtoMember(1)]
        private readonly string language;

        [ProtoMember(2)]
        private readonly string script;

        [ProtoMember(3)]
        private readonly string territory;

        [ProtoMember(4)]
        private readonly string variant;

        /// <summary>
        /// Unicode language subtag (also known as a Unicode base language code).
        /// </summary>
        public string Language => this.language;

        /// <summary>
        /// Unicode script subtag (also known as a Unicode script code).
        /// </summary>
        public string Script => this.script;

        /// <summary>
        /// Unicode region subtag (also known as a Unicode region code, or a Unicode territory code).
        /// </summary>
        public string Territory => this.territory;

        /// <summary>
        /// Unicode variant subtag (also known as a Unicode language variant code)
        /// </summary>
        public string Variant => this.variant;

        public CldrLocale()
        {
            
        }

        public CldrLocale(string language, string script = null, string territory = null, string variant = null)
        {
            this.language = language;
            this.script = script;
            this.territory = territory;
            this.variant = variant;
        }

        /// <summary>
        /// Gets the hyphen-separated code of this <see cref="CldrLocale"/>.
        /// </summary>
        public string Code => JoinNotNull("-", this.Language, this.Script, this.Territory, this.Variant);

        private static string JoinNotNull(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(x => x != null));
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrLocale;

            if (other == null)
                return false;

            return this.Code == other.Code;
        }

        public override int GetHashCode() => this.Code.GetHashCode();

        public static bool operator ==(CldrLocale left, CldrLocale right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CldrLocale left, CldrLocale right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => this.Code;
    }
}
