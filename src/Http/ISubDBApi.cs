using SubDBSharp.Http;
using System.Threading.Tasks;

namespace SubDBSharp
{
    /// <summary>
    /// Interface for interacting with the SubDB API to perform operations related to subtitle management.
    /// </summary>
    public interface ISubDBApi
    {
        /// <summary>
        /// Downloads a subtitle file based on the provided video hash and language preferences.
        /// </summary>
        /// <param name="hash">A unique hash representing the video file, generated using a specific hash function.</param>
        /// <param name="languages">A list of language codes indicating the preferred languages for the subtitle. The first matching language is returned.</param>
        /// <returns>A Task representing the asynchronous operation, containing the server's response to the subtitle download request.</returns>
        Task<Response> DownloadSubtitle(string hash, params string[] languages);

        /// <summary>
        /// Retrieves a list of available languages supported by the SubDB API for subtitles.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation, containing the server's response with the list of available languages.</returns>
        Task<Response> GetAvailableLanguagesAsync();

        /// <summary>
        /// Searches for subtitles based on the hash of a video file, optionally returning the number of subtitle versions available per language.
        /// </summary>
        /// <param name="hash">The hash of the video file, uniquely identifying it in the subtitle database.</param>
        /// <param name="getVersions">Optional parameter to indicate whether to include the number of versions available for each subtitle language.</param>
        /// <returns>A Task representing the asynchronous operation, containing the server's response to the subtitle search request.</returns>
        Task<Response> SearchSubtitle(string hash, bool getVersions = false);

        /// <summary>
        /// Uploads a subtitle file for a specific movie to the server.
        /// </summary>
        /// <param name="subtitle">The content of the subtitle file being uploaded.</param>
        /// <param name="movie">The movie identifier or hash that the subtitle is associated with.</param>
        /// <returns>A Task representing the asynchronous operation, containing the server's response to the upload request.</returns>
        Task<Response> UploadSubtitle(string subtitle, string movie);
    }
}