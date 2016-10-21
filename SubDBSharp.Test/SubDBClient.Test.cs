using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SubDBSharp.Test
{
    [TestClass]
    public class UnitTest1
    {
        static readonly Uri _sandBoxEndPoint = new Uri("http://sandbox.thesubdb.com/");

        [TestMethod]
        public void TestAvailableLanguages()
        {
            var client = new Client("SubDBSharp", "1.0", "https://github.com/ivandrofly/SubDBSharp");
            var sClient = new SubDBClient(client);
            string languagues = sClient.LanguagesAvailable();
            Assert.IsTrue(languagues.Length > 0);
        }

        [TestMethod]
        public void TestSearchSubtitle()
        {
            var client = new Client("SubDBSharp", "1.0", "https://github.com/ivandrofly/SubDBSharp");
            var sClient = new SubDBClient(client);
            string languagues = sClient.SearchSubtitle("ffd8d4aa68033dc03d1c8ef373b9028c");
            Assert.IsTrue(languagues.Length > 0);
        }

        [TestMethod]
        public void TestDownloadSubtitle()
        {
            var client = new Client("SubDBSharp", "1.0", "https://github.com/ivandrofly/SubDBSharp");
            var sClient = new SubDBClient(client);
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
            string movieFile = "";
            string subfile = "";
            string hash = Utils.GetHashString(movieFile);
            var client = new Client("SubDBSharp", "1.0", "https://github.com/ivandrofly/SubDBSharp");
            var sClient = new SubDBClient(_sandBoxEndPoint, client);
            string ouput = sClient.UploadSubtitle(movieFile, subfile);
        }

    }
}
