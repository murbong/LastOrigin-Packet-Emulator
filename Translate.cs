using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pcap
{
    public static class Translate
    {
        static Dictionary<string, string> TranslateDict = new Dictionary<string, string>();
        public static string GetTranslate(string source)
        {
            return TranslateDict.ContainsKey(source)? TranslateDict[source] : source;
        }
        public static void SetDictionary(string file)
        {
            var str = File.ReadAllLines(file);

            foreach(var i in str)
            {
                var split = i.Split(',');

                TranslateDict.Add(split[0], split[1]);
            }
        }
    }
}
