using Nasa.Models;
using Newtonsoft.Json;

namespace Nasa.Servicio
{
    public class NasaApiService
    {
        private readonly HttpClient _httpClient;

        public NasaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NasaResponse> GetImagesRoverForDateAsync(DateTime date, string? rover)
        {
            try
            {
                if (rover == null)
                {
                    rover = "curiosity";
                }
                var dateString = date.ToString("yyyy-MM-dd");

                var response = await _httpClient.GetAsync($"https://api.nasa.gov/mars-photos/api/v1/rovers/{rover}/photos?earth_date={dateString}&api_key=4KgyN5ddDkcazf6Xb3EMgzfFMm9en27RoUyyCxF7");

                var respuesta=response.IsSuccessStatusCode;

                if (!respuesta)
                {
                    return null;
                }
                var content = await response.Content.ReadAsStringAsync();

                var son= JsonConvert.DeserializeObject<NasaResponse>(content);
                return son;
            }
            catch(Exception ex)
            {
                throw;
            }
            

            //return photosResponse.Photos.Select(photo => photo.ImgSrc);
        }


        public async Task<NasaImage> GetImagesForDateAsync(DateTime date)
        {

            try
            {
                var url = $"https://api.nasa.gov/planetary/apod?api_key=4KgyN5ddDkcazf6Xb3EMgzfFMm9en27RoUyyCxF7&date={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();

                // Check if the response is an array of images
                /*if (json.StartsWith("["))
                {
                    var images = JsonConvert.DeserializeObject<List<NasaImage>>(json);
                    return images;
                }*/

                // Otherwise, the response is a single image
                var image = JsonConvert.DeserializeObject<NasaImage>(json);
                //return new List<NasaImage> { image };
                return image;
            }
            catch(Exception ex)
            {
                throw;
            }
            

            
        }

        public async Task<string> MostrarImagenDelDia()
        {
            var date=DateTime.Now;
            var url = $"https://api.nasa.gov/planetary/apod?api_key=4KgyN5ddDkcazf6Xb3EMgzfFMm9en27RoUyyCxF7&date={date:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var image = JsonConvert.DeserializeObject<NasaImage>(json);
            string urlImgDia = image.Url;
            return urlImgDia;
        }
    }
}
