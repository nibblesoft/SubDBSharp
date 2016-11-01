using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SubDBSharp.Test
{
    public class UnitTest
    {
        private static readonly Uri _sandBoxAddress = new Uri("http://sandbox.thesubdb.com/");

        [Fact]
        public void TestAvailableLanguages()
        {
            var subDbClient = new SubDBClient();
            IReadOnlyList<Language> languagues = subDbClient.LanguagesAvailable().Result;
            Assert.True(languagues.Count > 0);
        }

        [Fact]
        public void TestSearchSubtitle()
        {
            var subDbClient = new SubDBClient(_sandBoxAddress);
            Task<IReadOnlyList<Language>> languagues = subDbClient.SearchSubtitle("ffd8d4aa68033dc03d1c8ef373b9028c");
            Assert.True(languagues.Result.Count > 0);
        }

        [Fact]
        public void TestDownloadSubtitle()
        {
            Language[] languages = { new Language("en") };
            var subDbClient = new SubDBClient(_sandBoxAddress);
            var response = subDbClient.DownloadSubtitle("ffd8d4aa68033dc03d1c8ef373b9028c", languages);

            string file = "Subtitle-Downloaded.srt";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            using (var fs = new FileStream("Subtitle-Downloaded.srt", FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[1024];
                int bytesRead = 0;
                // TODO: Fix this hugly line...
                Stream stream = response.Result.StreamContent.ReadAsStreamAsync().Result;
                while ((bytesRead = stream.Read(buffer, 0, 1024)) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                }
            }
            Assert.True(File.Exists(file));
            File.Delete(file);
        }

        [Fact]
        public void TestUploadSubtitle()
        {
            string movieFile = @"D:\uTorrent\The Huntsman Winter's War (2016) [YTS.AG]\The.Huntsman.Winter's.War.2016.720p.BluRay.x264-[YTS.AG].mp4";
            string subfile = @"D:\uTorrent\The Huntsman Winter's War (2016) [YTS.AG]\The.Huntsman.Winter's.War.2016.720p.BluRay.x264-[YTS.AG].mp4";
            string hash = Utils.GetHashString(movieFile);
            var subDbClient = new SubDBClient(_sandBoxAddress);
            subDbClient.UploadSubtitle(movieFile, subfile);
        }

    }
}
