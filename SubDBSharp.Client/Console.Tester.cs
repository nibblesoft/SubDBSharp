using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp.ConsoleTest
{
    static class ConsoleTest
    {
        static readonly SubDBClient client = new SubDBClient(new Client("SubDBSharp", "1", "https://github.com/ivandrofly/SubDBSharp"));
        static void Main(string[] args)
        {
            UpdateSubtitleTest();
            // Main Thread wait here!!!
            Console.ReadLine();
        }

        static async /*Task */ void TestAvailableLanguages()
        {
            IReadOnlyList<Language> listLanguages = await client.LanguagesAvailable().ConfigureAwait(false);
            foreach (Language lang in listLanguages)
            {
                Console.WriteLine("Name: " + lang.Name);
            }
            Console.ReadLine();
        }

        static async /*Task */ void TestSearchSubtitle()
        {
            string hashString = Utils.GetHashString(@"D:\uTorrent\Westworld.S01E05.HDTV.x264-KILLERS[ettv]\Westworld.S01E05.HDTV.x264-KILLERS[ettv].mkv");
            IReadOnlyList<Language> listLanguages = await client.SearchSubtitle(hashString, true).ConfigureAwait(false);
            foreach (Language lang in listLanguages)
            {
                Console.WriteLine(lang);
            }
            Console.ReadLine();
        }

        static async /*Task */ void DownloadSubtitleTest()
        {
            var fi = new FileInfo(@"D:\uTorrent\Westworld.S01E05.HDTV.x264-KILLERS[ettv]\Westworld.S01E05.HDTV.x264-KILLERS[ettv].mkv");
            var language = new Language("en");
            string hashString = Utils.GetHashString(fi.FullName);
            Stream subtitleStream = await client.DownloadSubtitle(hashString, language).ConfigureAwait(false);
            using (var fs = new FileStream(fi.Name + language.Name + ".srt", FileMode.Create, FileAccess.Write))
            //using (var sr = new StreamWriter(fs, Encoding.UTF8))
            {
                subtitleStream.CopyTo(fs);
            }
            subtitleStream.Close();
            Console.ReadLine();
        }

        static async /*Task */ void UpdateSubtitleTest()
        {
            var fiMovie = new FileInfo(@"D:\uTorrent\Westworld.S01E05.HDTV.x264-KILLERS[ettv]\Westworld.S01E05.HDTV.x264-KILLERS[ettv].mkv");
            var fiSubtitle = new FileInfo(@"D:\uTorrent\Westworld.S01E05.HDTV.x264-KILLERS[ettv]\Westworld.S01E05.HDTV.x264-KILLERS[ettv].en.srt");
            bool succeded = await client.UploadSubtitle(fiMovie.FullName, fiSubtitle.FullName).ConfigureAwait(false);
            Console.WriteLine("Uploaded!");
        }

    }
}
