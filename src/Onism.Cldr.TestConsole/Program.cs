using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Tools;

namespace Onism.Cldr.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cldr.NET Test Console");
            Console.WriteLine("Doing stuff...");


            var path = @"\\Mac\Home\Desktop\";
            CldrPackage.Core.Download(path);


            Console.WriteLine();
            Console.WriteLine("Finished");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
