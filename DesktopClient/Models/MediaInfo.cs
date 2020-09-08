using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DesktopClient.Models
{
    public class MediaInfo
    {
        public string Hash { get; private set; }
        public FileInfo FileInfo { get; private set; }
        public IList<string> Subtitles { get; set; }

        public static MediaInfo FromFile(string file)
        {
            return new MediaInfo
            {
                Hash = SubDBSharp.Helpers.Utils.GetMovieHash(file),
                FileInfo = new FileInfo(file),
                Subtitles = LoadSubtitles(file)
            };
        }

        private static IList<string> LoadSubtitles(string file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            var list = new List<string>();
            foreach (var subtitleFile in Directory.GetFiles(Path.GetDirectoryName(file), $"{fileName}*.srt", SearchOption.TopDirectoryOnly))
            {
                list.Add(subtitleFile);
            }
            return list;
        }

    }
}
