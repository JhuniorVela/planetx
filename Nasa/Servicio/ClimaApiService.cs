using Microsoft.AspNetCore.Mvc;
using Nasa.Models;
using Newtonsoft.Json;

namespace Nasa.Servicio
{
    public class ClimaApiService
    {
        private readonly HttpClient _httpClient;

        public ClimaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherData> ObtenerClima(string city)
        {
            var apiKey = "287228a7ef7180ee3d359819dd68fe33";
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<WeatherData>(json);

            return data;
        }
    }
}
