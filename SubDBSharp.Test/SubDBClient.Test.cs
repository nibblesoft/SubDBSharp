using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SubDBSharp.Test
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly Uri _sandBoxAddress = new Uri("http://sandbox.thesubdb.com/");
        private static readonly Client _client = new Client("SubDBSharp", 1.0, "https://github.com/ivandrofly/SubDBSharp");

        [TestMethod]
        public void TestAvailableLanguages()
        {
            var sClient = new SubDBClient(_client, _sandBoxAddress);
            string languagues = sClient.LanguagesAvailable();
            Assert.IsTrue(languagues.Length > 0);
        }

        [TestMethod]
        public void TestSearchSubtitle()
        {
            var sClient = new SubDBClient(_client, _sandBoxAddress);
            string languagues = sClient.SearchSubtitle("ffd8d4aa68033dc03d1c8ef373b9028c");
            Assert.IsTrue(languagues.Length > 0);
        }

        [TestMethod]
        public void TestDownloadSubtitle()
        {
            var sClient = new SubDBClient(_client, _sandBoxAddress);
            Stream ms = sClient.DownloadSubtitle("ffd8d4aa68033dc03d1c8ef373b9028c", "en");
            using (var fs = new FileStream("Subtitle-Downloaded.srt", FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = ms.Read(buffer, 0, 1024)) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                }
            }
            Assert.IsTrue(File.Exists("Subtitle-Downloaded.srt"));
        }

        [TestMethod]
        public void TestUploadSubtitle()
        {
            string movieFile = @"D:\uTorrent\The Huntsman Winter's War (2016) [YTS.AG]\The.Huntsman.Winter's.War.2016.720p.BluRay.x264-[YTS.AG].mp4";
            string subfile = @"D:\uTorrent\The Huntsman Winter's War (2016) [YTS.AG]\The.Huntsman.Winter's.War.2016.720p.BluRay.x264-[YTS.AG].mp4";
            string hash = Utils.GetHashString(movieFile);
            var sClient = new SubDBClient(_client, _sandBoxAddress);
            sClient.UploadSubtitle(movieFile, subfile);
        }

    }
}
