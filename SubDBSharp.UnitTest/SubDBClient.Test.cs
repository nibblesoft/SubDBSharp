using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SubDBSharp.Helpers;
using Xunit;

namespace SubDBSharp.UnitTest
{
    public class UnitTest
    {
        private static readonly IResponseParser ReponseParser = new CsvResponseParser();

        [Fact(Skip = "Sandbox is down")]
        public async Task TestAvailableLanguages()
        {
            // NOTE: SINCE THE SANDBOX IS ALSO DOWN, USE MOCK API
            // TODO:
            //  * Use NSubstitue to mock the http client
            var subDbClient = new SubDBClient(GetProductInfo(), ApiUrls.SubDBApiSandBoxUrl);
            var response = await subDbClient.GetAvailableLanguagesAsync();
            Assert.NotNull(response.Body);

            var buffer = (byte[])response.Body;
            var body = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            var availableLanguages = ReponseParser.ParseGetAvailablesLanguages(body);

            Assert.True(availableLanguages.Count > 0);
        }

        [Fact(Skip = "Sandbox is down")]
        public async Task TestSearchSubtitle()
        {
            var subDbClient = new SubDBClient(GetProductInfo(), ApiUrls.SubDBApiSandBoxUrl);
            var response = await subDbClient.SearchSubtitleAsync("ffd8d4aa68033dc03d1c8ef373b9028c", false);
            Assert.NotNull(response.Body);
            var buffer = (byte[])response.Body;
            Assert.True(buffer.Length > 0);
            var body = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            var availableLanguages = ReponseParser.ParseGetAvailablesLanguages(body);
            Assert.True(availableLanguages.Count > 0);
        }

        [Fact(Skip = "Sandbox is down")]
        public async Task TestDownloadSubtitle()
        {
            var subDbClient = new SubDBClient(GetProductInfo(), ApiUrls.SubDBApiSandBoxUrl);
            var movieHash = Utils.GetMovieHash("./Assets/dexter.mp4");
            var response = await subDbClient.DownloadSubtitleAsync(movieHash, "en");
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact(Skip = "Sandbox is down")]
        public async Task TestUploadSubtitle()
        {
            var movie = "./Assets/dexter.mp4";
            var subtitle = @"./Assets/Logan.2017.en.srt";
            var subDbClient = new SubDBClient(GetProductInfo(), ApiUrls.SubDBApiSandBoxUrl);
            var response = await subDbClient.UploadSubtitleAsync(subtitle, movie);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);
        }

        private static ProductHeaderValue GetProductInfo() => new ProductHeaderValue("SubDBSharp.UnitTest", "1.0");
    }
}