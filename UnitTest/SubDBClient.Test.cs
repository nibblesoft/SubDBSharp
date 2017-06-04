using SubDbSharp;
using SubDbSharp.Http;
using SubDbSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        public static void TestSearchSubtitle()
        {
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            Task<IReadOnlyList<Language>> languagues = subDbClient.SearchSubtitleAsync("ffd8d4aa68033dc03d1c8ef373b9028c", false);
            Assert.True(languagues.Result.Count > 0);
        }

        [Fact]
        public async Task TestDownloadSubtitle()
        {
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            string movieHash = Utils.GetMovieHash("./Assets/dexter.mp4");
            var response = await subDbClient.DownloadSubtitleAsync(movieHash, "en");
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task TestUploadSubtitle()
        {
            string movie = "./Assets/dexter.mp4";
            string subtitle = @"./Assets/Logan.2017.en.srt";
            //string hash = ""
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            bool isUploaded = await subDbClient.UploadSubtitleAsync(subtitle, movie);
            Assert.True(isUploaded);
        }

        public static ProductHeaderValue GetProductInfo() => new ProductHeaderValue("UnitTest", "1.0");
    }
}
