using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;

namespace Onism.Cldr
{
    /// <summary>
    /// 
    /// </summary>
    public class CldrTree
    {
        private readonly CldrTreeNode _root;

        private readonly Dictionary<string, int> _values;

        private readonly Dictionary<string, int> _locales;

        public CldrTree()
        {
            _root = new CldrTreeNode(null);
            _values = new Dictionary<string, int>();
            _locales = new Dictionary<string, int>();
        }

        public CldrTreeNode SelectNode(string path) => _root.SelectNode(path);

        public void AddValue(string locale, string path, string value)
        {
            var pathData = path.Split('.');

            var localeId = _locales.GetOrAddId(locale);
            var valueId = _values.GetOrAddId(value);

            _root.Add(localeId, pathData, valueId);
        }
    }
}
