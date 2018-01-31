using SubDBSharp.Http;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public interface ISubDBClient
    {
        Task<Response> DownloadSubtitleAsync(string hash, params string[] languages);
        Task<Response> GetAvailableLanguagesAsync();
        Task<Response> SearchSubtitleAsync(string hash, bool getVersions);
        Task<Response> UploadSubtitleAsync(string subtitle, string movie);
    }
}