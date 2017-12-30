using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Waifu
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] Args)
        {
            string Board = "", SavePath = "";
            var ThreadNumber = 0;
            
            if (Args.Length >= 1)
            {
                Board = Args.FirstOrDefault(Arg => Arg.Length <= 4);
                Args.FirstOrDefault(Arg => int.TryParse(Arg, out ThreadNumber));
                if (Args.Any(Arg => Arg == "-d"))
                {
                    SavePath = Args.FirstOrDefault(Arg => Directory.Exists(Arg));
                }
                else
                {
                    using (var SaveFile = new FolderBrowserDialog())
                    {
                        if (SaveFile.ShowDialog() == DialogResult.OK)
                        {
                            SavePath = SaveFile.SelectedPath;
                        }
                        else
                        {
                            Console.WriteLine("No Save Location Specified... Exiting.");
                            Environment.Exit(1);
                        }
                    }
                }
            }
            else
            {
                Console.Write("what Is The Board ID?: ");
                Board = Console.ReadLine();

                Console.Write("What Is The Thread Number?: ");
                ThreadNumber = Convert.ToInt32(Console.ReadLine());

                Console.Write("Where Would You Like To Save Photos?: ");
                SavePath = Console.ReadLine();
            }

            if (SavePath == "")
            {
                SavePath = Environment.SpecialFolder.Desktop.ToString();
            }

            var Download = $"https://a.4cdn.org/{Board}/thread/{ThreadNumber}.json";
            Console.WriteLine($"Using: {Download}");
            Console.WriteLine($"Using Save Path {SavePath}");
            
            using (var Client = new WebClient())
            {
                var PreParsedData = Client.DownloadString(Download);
                var ParsedData = new JavaScriptSerializer().Deserialize<ThreadPosts>(PreParsedData);

                var NumberOfImagesFound = 0;
                foreach (var Post in ParsedData.Posts.Where(Post => Post.tim != null))
                {
                    if (Post.tim == null) continue;

                    NumberOfImagesFound++;
                    Console.Write($"Saving Image Number: {NumberOfImagesFound} From: ");
                    var DownloadPath = $"https://i.4cdn.org/{Board}/{Post.tim}{Post.ext}";
                    Console.WriteLine(DownloadPath);
                    try
                    {
                        Client.DownloadFile(DownloadPath, $"{SavePath}/{Post.filename}{Post.ext}");
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine($"Failed To Download {DownloadPath} {Ex.Message}");
                    }
                }
            }
        }
    }

    public class Post
    {
        public int no { get; set; }
        public string now { get; set; }
        public string name { get; set; }
        public string com { get; set; }
        public string filename { get; set; }
        public string ext { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int tn_w { get; set; }
        public int tn_h { get; set; }
        public object tim { get; set; }
        public int time { get; set; }
        public string md5 { get; set; }
        public int fsize { get; set; }
        public int resto { get; set; }
        public int bumplimit { get; set; }
        public int imagelimit { get; set; }
        public string semantic_url { get; set; }
        public int replies { get; set; }
        public int images { get; set; }
        public int unique_ips { get; set; }
    }

    public class ThreadPosts
    {
        public List<Post> Posts { get; set; }
    }
}