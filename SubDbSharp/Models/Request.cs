using SubDBSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SubDbSharp.Models
{
    public class Request
    {
        public string MovieHash { get; set; }
        public string Content { get; set; }

        public Request()
        {
        }

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
            MovieHash = Utils.GetHashString(movieFile);
            Content = File.ReadAllText(subtitleFile, Encoding.UTF8);
        }

    }
}
