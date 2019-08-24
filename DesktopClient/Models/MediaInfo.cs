using System.IO;

namespace DesktopClient.Models
{
    public class MediaInfo
    {
        public string Hash { get; private set; }
        public FileInfo FileInfo { get; private set; }

        public static MediaInfo FromFile(string file)
        {
            return new MediaInfo
            {
                Hash = SubDBSharp.Helpers.Utils.GetMovieHash(file),
                FileInfo = new FileInfo(file),
            };
        }

    }
}
