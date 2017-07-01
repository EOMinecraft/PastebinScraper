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
        private static string ParseArg(string[] args, string term)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-" + term)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
        static void Main(string[] args)
        {
            InitClient();
            Paste.categoryFilter = ParseArg(args, "c");
            Paste.contentFilter = ParseArg(args, "s");

            while (true)
            {
                try
                {
                    string dump = DumpRawArchive();
                    if (dump.Contains("You are scraping our site way too fast"))
                    {
                        Console.WriteLine("Maybe blocked?");
                    }
                    else
                    {
                        string tbl = TextHelper.StrBetweenStr(dump, "<table class=" + @"""" + "maintable" + @"""" + ">", "</table>");

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
                    }
                }
                catch
                {
                    Console.WriteLine("Unhandled error");
                }


                for (int i = 30; i >= 0; i--)
                {
                    if (i != 0)
                    {
                        Console.Write(i);
                        for (int ii = 0; ii < 3; ii++)
                        {
                            Console.Write(".");
                            Thread.Sleep(333);
                        }
                    }
                    else
                    {
                        Console.WriteLine(i);
                    }

                }
            }

        }

        private static string DumpRawArchive()
        {
            return client.DownloadString(PASTEBIN_ARCHIVE);
        }
    }
}
