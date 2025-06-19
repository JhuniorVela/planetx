using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nasa.configuracion;
using Nasa.Models;
using Newtonsoft.Json;

namespace Nasa.Servicio
{
    public class ClimaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _weatherApiSettings;

        public ClimaApiService(HttpClient httpClient, WeatherApiSettings weatherApiSettings)
        {
            _httpClient = httpClient;
            _weatherApiSettings = weatherApiSettings;
        }

        public async Task<WeatherData> ObtenerClima(string city)
        {
            try
            {
                var url = $"{_weatherApiSettings.Endpoint}?q={city}&appid={_weatherApiSettings.ApiKey}&units=metric";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WeatherData>(json);

                return data;
            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                throw new Exception($"Error al obtener datos del clima: {ex.Message}", ex);
            }
        }
    }
}
