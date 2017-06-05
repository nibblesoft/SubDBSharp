using SubDbSharp.Helpers;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace SubDbSharp.Test
{
    public class UnitTest
    {
        private static readonly Uri _sandBoxAddress = new Uri("http://sandbox.thesubdb.com/");
        private static readonly IResponseParser _reponseParser = new CsvResponseParser();

        [Fact]
        public async Task TestAvailableLanguages()
        {
            var subDbClient = new SubDbClient(GetProductInfo());
            var response = await subDbClient.GetLanguagesAvailableAsync();
            Assert.NotNull(response.Body);
            var availableLanguages = _reponseParser.ParseGetAvailablesLanguages(response.Body);
            Assert.True(availableLanguages.Count > 0);
        }

        [Fact]
        public async Task TestSearchSubtitle()
        {
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            var response = await subDbClient.SearchSubtitleAsync("ffd8d4aa68033dc03d1c8ef373b9028c", false);
            Assert.NotNull(response.Body);
            var availableLanguages = _reponseParser.ParseGetAvailablesLanguages(response.Body);
            Assert.True(availableLanguages.Count > 0);
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
            var subDbClient = new SubDbClient(GetProductInfo(), _sandBoxAddress);
            var response = await subDbClient.UploadSubtitleAsync(subtitle, movie);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);
        }

        public static ProductHeaderValue GetProductInfo() => new ProductHeaderValue("UnitTest", "1.0");
    }
}
