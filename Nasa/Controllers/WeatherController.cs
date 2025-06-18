using Microsoft.AspNetCore.Mvc;
using Nasa.Servicio;

namespace Nasa.Controllers
{
    public class WeatherController : Controller
    {
        private readonly ClimaApiService _climaService;

        public WeatherController(ClimaApiService climaService)
        {
            _climaService = climaService;
        }

        [HttpGet]
        public async Task<IActionResult> Clima()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Clima(string city)
        {
            var respuesta=await _climaService.ObtenerClima(city);
            ViewBag.City = city;
            return View(respuesta);
        }
    }
}
