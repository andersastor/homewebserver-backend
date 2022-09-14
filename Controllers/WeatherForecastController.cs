using homewebserver_backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace homewebserver_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly JsonFileWeatherForecastTimestampsService _jsonFileWeatherForecastTimestampsService;
        private readonly JsonFileWeatherForecastService _jsonFileWeatherForecastService;

        private readonly ProductInfoHeaderValue productName;
        private readonly ProductInfoHeaderValue commentValue;

        private readonly HttpClient httpClient;
        private readonly HttpRequestMessage httpRequest;

        public WeatherForecastController(
            IHttpClientFactory httpClientFactory,
            JsonFileWeatherForecastTimestampsService jsonFileWeatherForecastTimestampsService, 
            JsonFileWeatherForecastService jsonFileWeatherForecastService
        ) {
            _httpClientFactory = httpClientFactory;

            _jsonFileWeatherForecastTimestampsService = jsonFileWeatherForecastTimestampsService;
            _jsonFileWeatherForecastService = jsonFileWeatherForecastService;

            productName = new ProductInfoHeaderValue("homewebserver", "0.1.0");
            commentValue = new ProductInfoHeaderValue("(+https://github.com/andersastor)");

            httpRequest = this.CreateRequest();
            httpClient = _httpClientFactory.CreateClient();
            _jsonFileWeatherForecastService = jsonFileWeatherForecastService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<WeatherForecast> GetAsync()
        {
            WeatherForecastTimeStampVars? savedTimestamps = this.ReadForecastMetadataFromFile();
            WeatherForecast? weatherForecast = null;

            if (savedTimestamps == null)
            {
                weatherForecast = await this.SendRequest();
            }
            else
            {
                if (savedTimestamps.expires != null && savedTimestamps.lastModified != null)
                {
                    if (DateTimeOffset.Compare((DateTimeOffset)savedTimestamps.expires, DateTimeOffset.UtcNow) <= 0)
                    {
                        if (await this.CheckIfModifiedSince((DateTimeOffset)savedTimestamps.lastModified))
                        {
                            weatherForecast = await this.SendRequest();
                        }
                    }
                    else
                    {
                        weatherForecast = this.ReadWeatherForecastFromFile();
                    }
                }
                else
                {
                    // log error that expires/lastmodifed does not exist
                }
            }

            return weatherForecast ?? new WeatherForecast { };
        }

        private async Task<WeatherForecast?> SendRequest()
        {
            HttpResponseMessage response = await this.httpClient.SendAsync(this.httpRequest);
            WeatherForecast? weatherForecast = null;

            if (response.IsSuccessStatusCode)
            {
                this.WriteForecastMetadataToFile(response);

                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    this.WriteWeatherForecastToFile(response);
                }
            }
            return weatherForecast;
        }

        private WeatherForecast? ReadWeatherForecastFromFile()
        {
            return this._jsonFileWeatherForecastService.ReadFromFile();
        }

        private async void WriteWeatherForecastToFile(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStreamAsync();

            WeatherForecast? weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(content);
            string json = JsonSerializer.Serialize(weatherForecast);

            this._jsonFileWeatherForecastService.WriteToFile(json);
        }

        private WeatherForecastTimeStampVars? ReadForecastMetadataFromFile()
        {
            return this._jsonFileWeatherForecastTimestampsService.ReadFromFile();
        }

        private void WriteForecastMetadataToFile(HttpResponseMessage response)
        {
            WeatherForecastTimeStampVars timestamps = new();
            HttpContentHeaders headers = response.Content.Headers;
            timestamps.expires = headers.Expires;
            timestamps.lastModified = headers.LastModified;
            string timestampsJson = JsonSerializer.Serialize(timestamps);

            _jsonFileWeatherForecastTimestampsService.WriteToFile(timestampsJson);
        }

        private async Task<bool> CheckIfModifiedSince(DateTimeOffset lastModified)
        {
            HttpRequestMessage httpRequest = this.CreateRequest();
            httpRequest.Headers.IfModifiedSince = lastModified;
            HttpResponseMessage lastModifiedResponse = await this.httpClient.SendAsync(httpRequest);
            return lastModifiedResponse.IsSuccessStatusCode;
        }

        private HttpRequestMessage CreateRequest()
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=58.4109&lon=15.6216");
            httpRequest.Headers.UserAgent.Add(productName);
            httpRequest.Headers.UserAgent.Add(commentValue);
            return httpRequest;
        }
    }
}