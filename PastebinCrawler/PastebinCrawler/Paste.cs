using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PastebinCrawler
{
    public class Paste
    {
        public static List<Paste> pastes = new List<Paste>();
        public static string categoryFilter { get; set; }
        public static string contentFilter { get; set; }
        public string pid { get; set; }
        public string category { get; set; }
        public string title { get; set; }
        public string content { get; set; }

        private bool Exist(string id)
        {
            foreach (Paste paste in pastes)
            {
                if (paste.pid == id)
                {
                    return true;
                }
            }
            return false;
        }
        private void Delete(Paste paste)
        {
            
        }
        public Paste(string rawInput)
        {
            bool time = false, pidState = false;
            foreach (string line in TextHelper.GetLines(rawInput))
            {
                if (line.Contains("a href") && !pidState)
                {
                    // PID & Title
                    pid = TextHelper.StrBetweenStr(line, "<a href=" + @"""" + "/", @"""" + ">");
                    title = TextHelper.StrBetweenStr(line, pid + @"""" + ">", "</a></td>");
                    pidState = true;
                }
                else if (line.Contains("td_smaller h_800 td_right") && time)
                {
                    // Category
                    if (line.Contains("a href"))
                    {
                        // Named category
                        string[] cats = line.Split(new string[] { @"""" + ">" }, StringSplitOptions.None);
                        category = cats[cats.Count() - 1].Split('<')[0];
                    }
                    else
                    {
                        // No category
                        category = "-";
                    }
                }
                else
                {
                    //Timestamp
                    time = true;
                }
            }
            if (!Exist(pid) && pid.Trim(' ') != "")
            {
                System.Windows.Forms.MessageBox.Show(pid);
                if (categoryFilter != null && category.ToLower() != categoryFilter.ToLower())
                {
                    return;
                }
                content = Program.client.DownloadString(Program.PASTEBIN_URL + "raw/" +  pid);
                if (contentFilter != null && !content.ToLower().Contains(contentFilter.ToLower()))
                {
                    return;
                }
                pastes.Add(this);
            }
        }
    }
}
