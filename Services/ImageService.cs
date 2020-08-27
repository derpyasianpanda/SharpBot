using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpBot.Services
{
    public class ImageService
    {
        private readonly HttpClient _http;

        public ImageService(HttpClient http) => _http = http;

        public async Task<Stream> GetImage(string url) =>
            await (await _http.GetAsync(url)).Content.ReadAsStreamAsync();
    }
}