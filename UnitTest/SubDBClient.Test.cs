using SubDbSharp;
using SubDbSharp.Http;
using SubDbSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace SubDbSharp.Test
{
    public class UnitTest
    {
        private static readonly Uri _sandBoxAddress = new Uri("http://sandbox.thesubdb.com/");

        [Fact]
        public async void TestAvailableLanguages()
        {
            var subDbClient = new SubDbClient(GetProductInfo());
            IReadOnlyList<Language> languagues = await subDbClient.GetLanguagesAvailableAsync();
            Assert.True(languagues.Count > 0);
        }

        [Fact]
        public void TestSearchSubtitle()
        {
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            Task<IReadOnlyList<Language>> languagues = subDbClient.SearchSubtitleAsync("ffd8d4aa68033dc03d1c8ef373b9028c", false);
            Assert.True(languagues.Result.Count > 0);
        }

        [Fact]
        public void TestDownloadSubtitle()
        {
            //var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            //Task<Response> response = subDbClient.DownloadSubtitle("ffd8d4aa68033dc03d1c8ef373b9028c", "en");

            //string file = "Subtitle-Downloaded.srt";
            //if (File.Exists(file))
            //{
            //    File.Delete(file);
            //}
            //using (var fs = new FileStream("Subtitle-Downloaded.srt", FileMode.Create, FileAccess.Write))
            //{
            //    var buffer = new byte[1024];
            //    int bytesRead = 0;
            //    // TODO: Fix this hugly line...
            //    Stream stream = response.Result.SubStream;
            //    while ((bytesRead = stream.Read(buffer, 0, 1024)) > 0)
            //    {
            //        fs.Write(buffer, 0, bytesRead);
            //    }
            //}
            //Assert.True(File.Exists(file));
            //File.Delete(file);
        }

        [Fact]
        public async Task TestUploadSubtitle()
        {
            string file = @"./Assets/Logan.2017.en.srt";
            //string hash = ""
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            bool isUploaded = await subDbClient.UploadSubtitleAsync(file);
            Assert.True(isUploaded);
        }

        public static ProductHeaderValue GetProductInfo() => new ProductHeaderValue("UnitTest", "1.0");
    }
}
