using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onism.Cldr
{
    public class CldrDataBuilder
    {
        private readonly CldrJsonFileFinder fileFinder;
        private readonly CldrVersionConsistencyAssurer versionConsistencyAssurer;

        public CldrDataBuilder()
        {
            this.fileFinder = new CldrJsonFileFinder();
            this.versionConsistencyAssurer = new CldrVersionConsistencyAssurer();
        }

        public void Build(string directory)
        {
            foreach (var file in fileFinder.FindFiles(directory))
            {
                // versionConsistencyAssurer.AssureVersionIsConsistent();
            }
        }
    }
}
