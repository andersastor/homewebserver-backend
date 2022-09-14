using Newtonsoft.Json;

namespace homewebserver_backend.Services
{
    public class JsonFileWeatherForecastTimestampsService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string JsonFileName => Path.Combine(_webHostEnvironment.WebRootPath, "data", "weatherforecasttimestamps.json");

        public JsonFileWeatherForecastTimestampsService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        public void WriteToFile(string timestampsJson)
        {

            System.IO.File.WriteAllText(JsonFileName, timestampsJson);
        }

        public WeatherForecastTimeStampVars? ReadFromFile()
        {
            using StreamReader jsonFileReader = System.IO.File.OpenText(JsonFileName);
            string fileContent = jsonFileReader.ReadToEnd();

            WeatherForecastTimeStampVars? savedTimestamps = null;
            if (fileContent.Length > 0)
            {
                savedTimestamps = JsonConvert.DeserializeObject<WeatherForecastTimeStampVars>(fileContent);
            }
            return savedTimestamps;
        }
    }
}
