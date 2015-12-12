using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;
using Onism.Cldr.Tools;

namespace Onism.Cldr
{
    /*
// A tool to convert the JSON to a more compact format (allowing the person running it to specify a subset)
namespace Onism.Cldr.Tools
{
    public class JsonCompilerSettings
    {
        // some properties and stuff that is used to specify a subset of CLDR data
        //
    }

    public class JsonCompiler
    {
        public static byte[] Compile(string )
    }
}

    */

    // Some facts:
    // - the maximum depth of CLDR data is ~8
    // - there are ~19,300 leaves (out of ~22,800 vertices in total)
    // - there are ~6,262,000 key-value pairs in leaves (and they take about 85% of memory)
    // - counting separately for each leaf, there are only 654,000 distinct values in dictionaries
    // - about 50% of all the dictionaries in leaves contain data for all the locales

    /// <summary>
    /// 
    /// </summary>
    public class CldrTree
    {
        private readonly CldrTreeNode _root;

        private readonly Dictionary<string, int> _values;

        private readonly Dictionary<CldrLocale, int> _locales;

        internal CldrTree()
        {
            _root = new CldrTreeNode(null);
            _values = new Dictionary<string, int>();
            _locales = new Dictionary<CldrLocale, int>();
        }

        public CldrTreeNode SelectNode(string path) => _root.SelectNode(path);

        internal void Add(CldrJson cldrJson)
        {
            foreach (var leaf in cldrJson.Data.Leaves())
            {
                Add(cldrJson.Locale, leaf.Path, (string) leaf);
            }
        }

        private void Add(CldrLocale locale, string path, string value)
        {
            var pathData = path.Split('.');

            var localeId = _locales.GetOrAddId(locale);
            var valueId = _values.GetOrAddId(value);

            _root.Add(localeId, pathData, valueId);
        }
    }
}
