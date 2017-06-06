using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SubDBSharp.Models
{
    public class Request
    {
        public string MovieHash { get; set; }
        public string Content { get; set; }

        public Request(string movieFile, string subtitleFile)
        {
            if (File.Exists(movieFile))
            {
                throw new FileNotFoundException(movieFile);
            }
            if (File.Exists(subtitleFile))
            {
                throw new FileNotFoundException(movieFile);
            }
            MovieHash = Utils.GetMovieHash(movieFile);
            Content = File.ReadAllText(subtitleFile, Encoding.UTF8);
        }

    }
}
