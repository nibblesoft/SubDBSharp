using SubDBSharp.Helpers;
using System.IO;
using Xunit;

namespace SubDBSharp.Test
{
    public class UtilsTest
    {
        [Fact]
        public void TestGetHashString()
        {
            // dexter
            const string expected = "ffd8d4aa68033dc03d1c8ef373b9028c";
            // http://thesubdb.com/api/samples/dexter.mp4
            var path = "./Assets/dexter.mp4";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("dexter.mp4");
            }
            var hashString = Utils.GetMovieHash(path);
            Assert.Equal(expected, hashString);
        }
    }
}
