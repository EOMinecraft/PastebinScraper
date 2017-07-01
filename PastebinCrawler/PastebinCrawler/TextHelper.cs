using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastebinCrawler
{
    public static class TextHelper
    {
        public static string StrBetweenStr(string s, string term1, string term2)
        {
            string output = s;
            
            output = output.Split(new string[] { term1 }, StringSplitOptions.None)[1];
            output = output.Split(new string[] { term2 }, StringSplitOptions.None)[0];

            return output;
        }
        public static string[] GetLines(string s)
        {
            return s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
