using SubDBSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDbSharpConsole
{
    public class Movie
    {
        public bool HashSubtitles
        {
            get
            {
                return Subtitles?.Count > 0;
            }
        }
        public IList<string> Subtitles { get; }
        public FileInfo MovieInfo { get; set; }

        public Movie(string moviefile)
        {
            Subtitles = new List<string>();
            MovieInfo = new FileInfo(moviefile);
            LoadSubtitles(MovieInfo);
        }

        private void LoadSubtitles(FileInfo mi)
        {
            string ext = mi.Extension;
            string movieNoExtension = Path.GetFileNameWithoutExtension(mi.Name);
            foreach (FileInfo fi in mi.Directory.GetFiles("*.srt"))
            {
                if (fi.Name.StartsWith(movieNoExtension, StringComparison.OrdinalIgnoreCase) && fi.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase) == false)
                {
                    Subtitles.Add(fi.FullName);
                }
            }
        }
    }
    public class MoveDirectory
    {
        private static readonly HashSet<string> _movieExtensions = new HashSet<string> { ".mp4", ".avi", ".mkv" };
        public bool HashSubtitles { get; private set; }
        public string MovieDirectory { get; set; }

        public IList<Movie> Movies { get; }

        public MoveDirectory(string directory)
        {
            Movies = new List<Movie>();
            foreach (var file in Directory.GetFiles(directory))
            {
                if (IsMovie(file))
                {
                    Movies.Add(new Movie(file));
                }
            }
        }



        private bool IsMovie(string file)
        {
            string ext = Path.GetExtension(file).ToLowerInvariant();
            return _movieExtensions.Contains(ext);
        }
    }

    class Program
    {
        static SubDBClient client = new SubDBClient(new Client("SubDBSharp", "1", "https://github.com/ivandrofly/SubDBSharp"));
        private static MoveDirectory _movieDirectory;
        static void Main(string[] args)
        {
#if false
            foreach (var item in Directory.GetDirectories(@"G:\Medias\Tv-Shows\Continuum"))
            {
                DownloadAllInFolder(item);
            }
#else
            DownloadAllInFolder(@"G:\Medias\Tv-Shows\Supernatural\Supernatural - Season 1");
            // UploadAll(@"G:\Medias\Tv-Shows\Narcos\Narcos S02 720p Complete");

#endif
            Console.WriteLine("All done!");
            Console.ReadLine();
        }

        public static void UploadAll(string path)
        {
            //string hash = Utils.GetHashString(movieFile);
            var client = new SubDBClient(new Client("SubDBSharp", "1", "https://github.com/ivandrofly/SubDBSharp"));
            var movieDirectory = new MoveDirectory(path);
            _movieDirectory = new MoveDirectory(path);
            foreach (var movie in _movieDirectory.Movies)
            {
                foreach (var subtitle in movie.Subtitles)
                {
                    try
                    {
                        if (movie.HashSubtitles)
                        {
                            Console.WriteLine($"Uploading: {movie.MovieInfo.Name}");
                            client.UploadSubtitle(movie.MovieInfo.FullName, subtitle);
                            Console.WriteLine($"{movie.MovieInfo.Name} was uploaded with success.");
                            Console.WriteLine();
                        }
                    }
                    catch (System.Net.WebException ex)
                    {
                        Console.WriteLine($"{ex.Message}/{ex.Status.ToString()} {movie.MovieInfo.Name} already exists!");
                        Console.WriteLine();
                        continue;
                    }
                }
            }
        }

        public static void DownloadAllInFolder(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var fi = new FileInfo(file);
                if (fi.Extension == ".mp4" || fi.Extension == ".mkv" || fi.Extension == ".avi")
                {
                    var languages = new string[] { "en", "pt" };
                    string moviehash = Utils.GetHashString(fi.FullName);
                    foreach (var lang in languages)
                    {
                        try
                        {
                            string name = string.Format("{0}-{1}.srt", Path.GetFileNameWithoutExtension(fi.Name), lang);
                            using (Stream content = client.DownloadSubtitle(moviehash, lang))
                            using (var fs = new FileStream(Path.Combine(fi.DirectoryName, name), FileMode.Create, FileAccess.Write))
                            {
                                var buffer = new byte[1024];
                                int bytesRead = 0;
                                while ((bytesRead = content.Read(buffer, 0, 1024)) > 0)
                                {
                                    fs.Write(buffer, 0, bytesRead);
                                }
                            }
                            Console.WriteLine($"Downloaded: {name}({lang})");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                }
            }
        }

    }
}
