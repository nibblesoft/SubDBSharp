using System.IO;

namespace DesktopClient.Models
{
    public class MediaInfo
    {
        public string Hash { get; }
        public FileInfo FileInfo { get; }

        public MediaInfo(string file)
        {
            Hash = SubDBSharp.Utils.GetMovieHash(file);
            FileInfo = new FileInfo(file);
        }
    }
}
