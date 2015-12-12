using System;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Packages
{
    /// <summary>
    /// Represents a single CLDR JSON file. 
    /// </summary>
    internal sealed class CldrJson
    {
        /// <summary>
        /// Gets the type of the package this file was extracted from.
        /// </summary>
        public Type Package { get; }

        /// <summary>
        /// Gets the locale this file is associated with.
        /// </summary>
        public CldrLocale Locale { get; }

        /// <summary>
        /// Gets the actual data stored in this file.
        /// </summary>
        public JObject Data { get; }

        public CldrJson(Type package, CldrLocale locale, JObject data)
        {
            Package = package;
            Locale = locale;
            Data = data;
        }
    }
}
