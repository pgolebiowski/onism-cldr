using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Onism.Cldr
{
    public class CldrLocale
    {
        public string LocaleCode { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "territory")]
        public string Territory { get; set; }

        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }

        [JsonProperty(PropertyName = "variant")]
        public string Variant { get; set; }
    }
}
