using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpsonsDownloader
{
    class Program
    {
        private static string sipsonsSite = "http://simpsons.fox-fan.ru/";
        private static string pathForDownloading = "D:\\Movies\\The Simpsons";
        private static string seasonNumber, episodeNumber, pageSource, SubtitleFileExtension, VideoFileExtension;
        private static string HtmlPagePath = Path.Combine(Environment.CurrentDirectory, "page.html");

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Set the season and episode numbers. For ex. SimpsonsDownloader s4 e15");
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Set the season and episode numbers. There must be two parameters. For ex. SimpsonsDownloader s4 e15");
            }
            else if (args.Length > 2)
            {
                Console.WriteLine("Too much parameters. Set the season and episode numbers. There must be two parameters. For ex. SimpsonsDownloader s4 e15");
            }
            else
            {
                if (args[0].Equals("0") || args[1].Equals("0") || args[0].Equals("00") || args[1].Equals("00"))
                    Console.WriteLine("There is no season " + args[0] + " or episode " + args[0] + ". Set the correct season and episode numbers. There must be two parameters. For ex. SimpsonsDownloader s4 e15");

                try
                {
                    //getPageSource(getSeasonNumber(args[0]), getEpisodeNumber(args[1]));
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //downloadVideoFileByUrl(parsePageSourceAndGetVideoFileUrl(HtmlPagePath, Encoding.UTF8));
            //downloadSubtitleFileByUrl(parsePageSourceAndGetSubTitleFileUrl(HtmlPagePath, Encoding.UTF8));

        }

        private static string getSeasonNumber(string argument)
        {
            return argument.Substring(1, argument.Length - 1);
        }

        private static string getEpisodeNumber(string argument)
        {
            return argument.Substring(1, argument.Length - 1);
        }

        private static void getPageSource(string seasonNumber, string episodeNumber)
        {
            if (episodeNumber.Length == 1)
            {
                if (Int32.Parse(episodeNumber) <= 9) episodeNumber = "0" + episodeNumber;

            }

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
                StreamWriter writer = new StreamWriter(HtmlPagePath);

                writer.WriteLine(pageSource);
                response.Close();
                readStream.Close();
            }
        }

        private static string parsePageSourceAndGetVideoFileUrl(string pageSourceFile, Encoding encoding)
        {
            string wholeDocumentString;
            string patternForVideoFileUrl = "http[^,]*\\.mp4";

            using (StreamReader streamReader = new StreamReader(pageSourceFile, encoding))
            {
                wholeDocumentString = streamReader.ReadToEnd();
            }

            Match result = Regex.Match(wholeDocumentString, patternForVideoFileUrl);

            return result.ToString();
        }

        private static string parsePageSourceAndGetSubTitleFileUrl(string pageSourceFile, Encoding encoding)
        {
            string wholeDocumentString;
            string patternForVideoFileUrl = "'st':'http.*?'";

            using (StreamReader streamReader = new StreamReader(pageSourceFile, encoding))
            {
                wholeDocumentString = streamReader.ReadToEnd();
            }

            Match result = Regex.Match(wholeDocumentString, patternForVideoFileUrl);

            return result.ToString().Substring(6, result.ToString().Length - 7);
        }

        private static void downloadVideoFileByUrl(string videoFileUrl)
        {
            WebClient webClient;
            Stopwatch stopWatch = new Stopwatch();

            VideoFileExtension = videoFileUrl.Substring(videoFileUrl.LastIndexOf(".")).Trim();

            using (webClient = new WebClient())
            {
                //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                //Uri URL = videoFileUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
                webClient.DownloadFile(videoFileUrl, pathForDownloading + "\\The Simpons s" + seasonNumber + "_e" + episodeNumber + VideoFileExtension);
            }
        }

        private static void downloadSubtitleFileByUrl(string subTitleFileUrl)
        {
            WebClient webClient;

            SubtitleFileExtension = subTitleFileUrl.Substring(subTitleFileUrl.LastIndexOf(".")).Trim();

            using (webClient = new WebClient())
            {
                webClient.DownloadFile(subTitleFileUrl, pathForDownloading + "\\The Simpons s" + seasonNumber + "_e" + episodeNumber + SubtitleFileExtension);
            }
        }
    }
}
