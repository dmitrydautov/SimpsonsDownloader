using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SimpsonsDownloader
{
    class Program
    {
        private static string sipsonsSite = "http://simpsons.fox-fan.ru/";
        private static string pathForDownloading = "D:\\Movies\\The Simpsons";
        private static string seasonNumber;
        private static string episodeNumber;
        private static string pageSource;
        static string testPage = "page.html";
        static string testPagePath = Path.Combine(Environment.CurrentDirectory, testPage);

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Set the season and episode numbers");
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Set the season and episode numbers. There must be two parameters");
            }
            else
            {
                try
                {
                    seasonNumber = args[0].Substring(1, args[0].Length - 1);
                    episodeNumber = args[1].Substring(1, args[1].Length - 1);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            getPageSource(seasonNumber, episodeNumber);
        }
        private static void getPageSource(string seasonNumber, string episodeNumber)
        {
            string definedUrlToSimpsonsSite = sipsonsSite + "series.php?id=" + seasonNumber + episodeNumber + "&voice=8";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(definedUrlToSimpsonsSite);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                pageSource = readStream.ReadToEnd();
                Console.WriteLine(pageSource);
                Thread.Sleep(1000);
                StreamWriter writer = new StreamWriter(testPagePath);

                writer.WriteLine(pageSource);
                response.Close();
                readStream.Close();
            }
            //return pageSource;
        }
    }
}
