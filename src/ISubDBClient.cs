using System.Threading.Tasks;
using SubDBSharp.Http;

namespace SubDBSharp
{
    /// <summary>
    /// Defines methods for interacting with the SubDB API,
    /// including operations for downloading, uploading, searching subtitles,
    /// and retrieving available subtitle languages.
    /// </summary>
    public interface ISubDBClient
    {
        /// <summary>
        /// Downloads the subtitle for a given video file based on its hash and specified language preferences.
        /// </summary>
        /// <param name="hash">The hash of the video file, used to uniquely identify the video.</param>
        /// <param name="languages">An array of language codes specifying the preferred subtitle language(s).
        /// If multiple language codes are provided, the first available subtitle will be returned in the specified order.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server response,
        /// including the subtitle content and its associated metadata.</returns>
        Task<Response> DownloadSubtitleAsync(string hash, params string[] languages);

        /// <summary>
        /// Retrieves the list of all available subtitle languages currently supported in the SubDB database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server response,
        /// including the list of available language codes.</returns>
        Task<Response> GetAvailableLanguagesAsync();

        /// <summary>
        /// Searches for subtitles for a given video file based on its hash.
        /// Optionally, retrieves additional information about the subtitle versions available.
        /// </summary>
        /// <param name="hash">The hash of the video file used to uniquely identify the video.</param>
        /// <param name="getVersions">A boolean indicating whether to return information about the number of subtitle versions per language available in the database.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server response, including available subtitles and their metadata if found.</returns>
        Task<Response> SearchSubtitleAsync(string hash, bool getVersions);

        /// <summary>
        /// Uploads a subtitle file for a specific movie to the SubDB server.
        /// </summary>
        /// <param name="subtitle">The subtitle content to be uploaded.</param>
        /// <param name="movie">The movie name or identifier associated with the subtitle.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server response,
        /// including status and any additional metadata related to the upload operation.</returns>
        Task<Response> UploadSubtitleAsync(string subtitle, string movie);
    }
}