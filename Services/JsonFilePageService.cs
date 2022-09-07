using homewebserver_backend.Models;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace homewebserver_backend.Services
{
    public class JsonFilePageService
    {
        private IWebHostEnvironment WebHostEnvironment;

        public JsonFilePageService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        private string JsonFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "pages.json");

        public IEnumerable<Page> GetPages()
        {
            using StreamReader JsonFileReader = File.OpenText(JsonFileName);
            return JsonSerializer.Deserialize<Page[]>(JsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
