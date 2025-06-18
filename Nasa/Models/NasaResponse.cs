using Newtonsoft.Json;

namespace Nasa.Models
{
    public class NasaResponse
    {
        [JsonProperty("photos")]
        public List<NasaPhoto> Photos { get; set; }
        
    }

    /*public class NasaPhoto
    {
        [JsonProperty("img_src")]
        public string ImgSrc { get; set; }
        public int id { get; set; }
        public string earth_date { get; set; }
    }*/

    /*public class Water
    {
        [JsonProperty("photos")]
        public List<Photo> Photos { get; set; }
    }*/

    public class NasaPhoto
    {
        public int PageIndex { get; set; }
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sol")]
        public long Sol { get; set; }

        [JsonProperty("camera")]
        public Camera Camera { get; set; }

        [JsonProperty("img_src")]
        public Uri ImgSrc { get; set; }

        public DateTimeOffset earth_date { get; set; }

        [JsonProperty("rover")]
        public Rover Rover { get; set; }
    }

    public class Camera
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rover_id")]
        public long RoverId { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }
    }

    public class Rover
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("landing_date")]
        public DateTimeOffset LandingDate { get; set; }

        [JsonProperty("launch_date")]
        public DateTimeOffset LaunchDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class NasaImage
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public string Url { get; set; }
        public string MediaType { get; set; }

        [JsonProperty("hdurl")]
        public string HdUrl { get; set; }


    }

}
