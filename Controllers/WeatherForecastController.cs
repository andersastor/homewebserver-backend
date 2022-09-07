using homewebserver_backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using System.Net.Http.Headers;
using System.Text.Json;

namespace homewebserver_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ProductInfoHeaderValue productName;
        private readonly ProductInfoHeaderValue commentValue;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;

            productName = new ProductInfoHeaderValue("homewebserver", "0.1.0");
            commentValue = new ProductInfoHeaderValue("(+https://github.com/andersastor)");
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            /*TODO:
             x initialize sitename var
             x construct https request
             x generate user-agent object using sitename
             x call request
             store expires and last-modified headers for later use
             if result is 2XX success parse body to JSON and save to file
             for subsequent requests:
                check if current time > expires
                if truesend new request with if-modified-since
                if 200 OK response, send new request
             https://developer.yr.no/doc/locationforecast/HowTO/ 
             */


            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=58.4109&lon=15.6216");
            httpRequest.Headers.UserAgent.Add(productName);
            httpRequest.Headers.UserAgent.Add(commentValue);

            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStreamAsync();

            WeatherForecast? weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(content);
            string json = JsonSerializer.Serialize(weatherForecast);
            System.IO.File.WriteAllText(Path.Combine(_webHostEnvironment.WebRootPath, "data", "weatherforecast.json"), json);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {})
            .ToArray();
        }
    }
}