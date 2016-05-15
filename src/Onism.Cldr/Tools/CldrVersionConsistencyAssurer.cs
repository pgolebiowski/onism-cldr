using System;

namespace Onism.Cldr.Tools
{
    internal class CldrVersionConsistencyAssurer
    {
        private string version;
        private string jsonPath;

        public string Version => this.version ?? "unspecified";

        public void AssureVersionIsConsistent(string version, string jsonPath)
        {
            if (this.version == null)
            {
                this.version = version;
                this.jsonPath = jsonPath;
                return;
            }

            if (string.Equals(this.version, version))
            {
                this.jsonPath = jsonPath;
                return;
            }

            var first = $"Version '{this.version}' was found in the file '{this.jsonPath}'.";
            var second = $"Version '{version}' was found in the file '{jsonPath}'.";
            throw new FormatException($"Inconsistent CLDR versions detected.\n{first}\n{second}");
        }
    }
}
