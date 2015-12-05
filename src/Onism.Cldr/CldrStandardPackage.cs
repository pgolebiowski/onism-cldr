namespace Onism.Cldr
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

        private bool _isFullVariant = true;

        /// <summary>
        /// Gets a flag indicating whether this package variant is "full".
        /// False means the variant is "modern". The default value is true.
        /// </summary>
        public bool IsFullVariant => _isFullVariant;

        /// <summary>
        /// Sets this package to be of the "full" variant.
        /// </summary>
        public CldrStandardPackage AsFull()
        {
            if (_isFullVariant == false)
            {
                Name = Name.Replace("-modern", "-full");
                _isFullVariant = true;
            }

            // for chaining
            return this;
        }

        /// <summary>
        /// Sets this package to be of the "modern" variant.
        /// </summary>
        public CldrStandardPackage AsModern()
        {
            if (_isFullVariant)
            {
                Name = Name.Replace("-full", "-modern");
                _isFullVariant = false;
            }

            // for chaining
            return this;
        }
    }
}