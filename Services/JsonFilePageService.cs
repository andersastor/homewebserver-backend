using homewebserver_backend.Models;
using Newtonsoft.Json;

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
            using StreamReader jsonFileReader = File.OpenText(JsonFileName);
            string fileContent = jsonFileReader.ReadToEnd();
            Page[]? pageList = null;

            if (fileContent.Length > 0)
            {
                pageList = JsonConvert.DeserializeObject<Page[]>(fileContent);
            }
            return pageList != null ? pageList : Array.Empty<Page>();
        }
    }
}
