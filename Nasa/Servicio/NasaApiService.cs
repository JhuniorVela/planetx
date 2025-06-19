using Microsoft.Extensions.Options;
using Nasa.configuracion;
using Nasa.Models;
using Newtonsoft.Json;

namespace Nasa.Servicio
{
    public class NasaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly NasaApiSettings _nasaApiSettings;
        public NasaApiService(HttpClient httpClient, NasaApiSettings nasaApiSettings)
        {
            _httpClient = httpClient;
            _nasaApiSettings = nasaApiSettings;
        }

        public async Task<NasaResponse> GetImagesRoverForDateAsync(DateTime date, string? rover = null)
        {
            try
            {
                rover ??= "curiosity"; // Forma más limpia de asignar valor por defecto

                var dateString = date.ToString("yyyy-MM-dd");
                var url = $"{_nasaApiSettings.MarsPhotosEndpoint}{rover}/photos?earth_date={dateString}&api_key={_nasaApiSettings.ApiKey}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<NasaResponse>(content);

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error al obtener imágenes del rover: {ex.Message}", ex);
            }
        }

        public async Task<NasaImage> GetImagesForDateAsync(DateTime date)
        {
            try
            {
                var url = $"{_nasaApiSettings.ApodEndpoint}?api_key={_nasaApiSettings.ApiKey}&date={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var image = JsonConvert.DeserializeObject<NasaImage>(json);

                return image;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error al obtener imagen del día: {ex.Message}", ex);
            }
        }

        public async Task<string> MostrarImagenDelDia()
        {
            try
            {
                var date = DateTime.Now;
                var image = await GetImagesForDateAsync(date);

                return image?.Url;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al mostrar imagen del día: {ex.Message}", ex);
            }
        }
    }
}
