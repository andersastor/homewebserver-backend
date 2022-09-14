using Newtonsoft.Json;

namespace homewebserver_backend.Services
{
    public class JsonFileWeatherForecastService
    {
        private readonly IWebHostEnvironment WebHostEnvironment;

        public JsonFileWeatherForecastService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        private string JsonFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "weatherforecast.json");

        public void WriteToFile(string weatherForecastJson)
        {
            System.IO.File.WriteAllText(JsonFileName, weatherForecastJson);
        }

        public WeatherForecast? ReadFromFile()
        {
            using StreamReader jsonFileReader = System.IO.File.OpenText(JsonFileName);
            string fileContent = jsonFileReader.ReadToEnd();

            WeatherForecast? savedWeatherforecast = null;
            if (fileContent.Length > 0)
            {
                savedWeatherforecast = JsonConvert.DeserializeObject<WeatherForecast>(fileContent);
            }

            return savedWeatherforecast;
        }
    }
}
