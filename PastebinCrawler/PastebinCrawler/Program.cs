using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using PastebinCrawler;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PastebinCrawler
{
    class Program
    {
        internal const string PASTEBIN_URL = "https://pastebin.com/";
        private const string PASTEBIN_ARCHIVE = PASTEBIN_URL + "archive";
        private static string SAVE_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\pastes\\";
        internal static WebClient client = null;
        private static List<Paste> pastes = new List<Paste>();

        private static void InitClient()
        {
            if (client == null)
            {
                client = new WebClient();
                client.Proxy = null;
            }
        }

        static void Main(string[] args)
        {
            InitClient();

            while (true)
            {
                string tbl = TextHelper.StrBetweenStr(DumpRawArchive(), "<table class=" + @"""" + "maintable" + @"""" + ">", "</table>");
                string[] records = tbl.Split(new string[] { "<td>" }, StringSplitOptions.None);

                foreach (string record in records)
                {
                    new Paste(record);
                }
                Console.WriteLine("Pastes: " + Paste.pastes.Count);

                foreach (Paste paste in Paste.pastes)
                {

                    string buildPath = SAVE_LOCATION + paste.category + "\\";
                    string buildFile = buildPath + paste.pid + ".txt";
                    if (!Directory.Exists(buildPath))
                    {
                        Directory.CreateDirectory(buildPath);
                    }

                    if (!File.Exists(buildPath))
                    {
                        File.WriteAllText(buildFile, paste.content);
                    }
                }

                for (int i = 10; i >= 0; i--)
                {
                    if (i != 0)
                    {
                        Console.Write(i + ", ");
                    }
                    else
                    {
                        Console.WriteLine(i);
                    }
                    Thread.Sleep(1000);
                }
            }
            
        }

        private static string DumpRawArchive()
        {
            return client.DownloadString(PASTEBIN_ARCHIVE);
        }
    }
}
