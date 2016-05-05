using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents a single CLDR JSON file. 
    /// </summary>
    public sealed class CldrJson
    {
        /// <summary>
        /// Gets the locale this file is associated with.
        /// </summary>
        public CldrLocale Locale { get; }

        /// <summary>
        /// Gets the actual data stored in this file.
        /// </summary>
        public JObject Data { get; }

        public CldrJson(CldrLocale locale, JObject data)
        {
            Locale = locale;
            Data = data;
        }
    }
}
