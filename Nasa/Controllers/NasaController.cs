using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nasa.Models;
using Nasa.Servicio;
using System.Text.Json;

namespace Nasa.Controllers
{
    public class NasaController : Controller
    {
        private readonly NasaApiService _nasaApiService;
        private readonly ILogger<NasaController> _logger;

        public NasaController(NasaApiService nasaApiService)
        {
            _nasaApiService= nasaApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Apod()
        {
            try
            {
                string imgDia = await _nasaApiService.MostrarImagenDelDia();

                ViewBag.ImgDia = imgDia;

            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message + " no se a podido conectar con el API";
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Apod(DateTime? date)
        {
            try
            {
                DateTime selectedDate = date ?? DateTime.Today;
        
                DateTime minDate = new DateTime(1995, 6, 16);
                if (selectedDate < minDate || selectedDate > DateTime.Today)
                {
                    ViewData["ErrorMessage"] = "Fecha fuera de rango";
                    ViewData["ErrorDetails"] = $"Por favor selecciona una fecha entre {minDate:dd/MM/yyyy} y {DateTime.Today:dd/MM/yyyy}";
                    ViewData["ErrorResolution"] = "Se usará la imagen del día actual";

                    selectedDate = DateTime.Today;
                }

                NasaImage model = await _nasaApiService.GetImagesForDateAsync(selectedDate);
                ViewBag.ImgDia = await _nasaApiService.MostrarImagenDelDia();

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                // Manejo específico para errores de red/API
                ViewData["ErrorMessage"] = "Error de conexión con la NASA API";
                ViewData["ErrorDetails"] = "No pudimos establecer conexión con los servidores de la NASA";
                ViewData["ErrorResolution"] = "Por favor verifica tu conexión a internet e intenta nuevamente";
        
                _logger.LogError(ex, "Error de conexión con NASA API");
                return View();
            }
            catch (JsonException ex)
            {
                // Manejo específico para errores en el formato de datos
                ViewData["ErrorMessage"] = "Error en el formato de datos";
                ViewData["ErrorDetails"] = "La NASA API devolvió datos en un formato inesperado";
                ViewData["ErrorResolution"] = "Por favor intenta nuevamente más tarde";
        
                _logger.LogError(ex, "Error de formato de datos de NASA API");
                return View();
            }
            catch (Exception ex)
            {
                // Manejo genérico para cualquier otro error
                ViewData["ErrorMessage"] = "Error inesperado";
                ViewData["ErrorDetails"] = $"Ocurrió un problema: {ex.Message}";
                ViewData["ErrorResolution"] = "Por favor intenta nuevamente o contacta al soporte técnico";
        
                _logger.LogError(ex, "Error inesperado en Apod");
                return View();
            }
        }

        public IActionResult Rover()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Rover(DateTime date, string? rover, int pageNumber)
        {
            if (date == null || string.IsNullOrWhiteSpace(rover))
            {
                return View(new List<NasaPhoto>());
            }
            const int pageSize = 10;

            try
            {
                if (string.IsNullOrWhiteSpace(rover))
                {
                    throw new ArgumentException("Debe seleccionar un rover válido");
                }

                DateTime minDate;
                switch (rover.ToLower())
                {
                    case "curiosity":
                        minDate = new DateTime(2012, 8, 6);
                        break;
                    case "opportunity":
                        minDate = new DateTime(2004, 1, 26);
                        break;
                    case "spirit":
                        minDate = new DateTime(2004, 1, 5);
                        break;
                    default:
                        throw new ArgumentException("Rover no reconocido");
                }

                if (date < minDate || date > DateTime.Today)
                {
                    ViewData["ErrorMessage"] = "Fecha fuera de rango";
                    ViewData["ErrorDetails"] = $"El rover {rover.ToUpper()} solo tiene imágenes entre {minDate:dd/MM/yyyy} y {DateTime.Today:dd/MM/yyyy}";
                    ViewData["ErrorResolution"] = "Selecciona una fecha dentro del rango válido";

                    if (date < minDate) date = minDate;
                    if (date > DateTime.Today) date = DateTime.Today;
                }

                var images = await _nasaApiService.GetImagesRoverForDateAsync(date, rover);

                if (images == null || images.Photos == null || !images.Photos.Any())
                {
                    ViewData["ErrorMessage"] = "No hay imágenes disponibles";
                    ViewData["ErrorDetails"] = $"No se encontraron imágenes para el rover {rover.ToUpper()} en la fecha {date:dd/MM/yyyy}";
                    ViewData["ErrorResolution"] = "Intenta con otra fecha o selecciona un rover diferente";
                    return View(new List<NasaPhoto>());
                }

                // Paginación
                var list = images.Photos;
                int totalItems = list.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

                int skip = (pageNumber - 1) * pageSize;
                List<NasaPhoto> paginatedList = list.Skip(skip).Take(pageSize).ToList();

                ViewData["CurrentPage"] = pageNumber;
                ViewData["TotalPages"] = totalPages;
                ViewData["Rover"] = rover;
                ViewData["Date"] = date.ToString("yyyy-MM-dd");
                ViewData["TotalImages"] = totalItems;

                return View(paginatedList);
            }
            catch (ArgumentException ex)
            {
                // Manejo de errores de parámetros inválidos
                ViewData["ErrorMessage"] = "Datos de entrada inválidos";
                ViewData["ErrorDetails"] = ex.Message;
                ViewData["ErrorResolution"] = "Por favor verifica los datos e intenta nuevamente";

                _logger.LogWarning(ex, "Argumento inválido en Rover");
                return View(new List<NasaPhoto>());
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de conexión con la API
                ViewData["ErrorMessage"] = "Error de conexión con la NASA API";
                ViewData["ErrorDetails"] = "No pudimos establecer conexión con los servidores de la NASA";
                ViewData["ErrorResolution"] = "Por favor verifica tu conexión a internet e intenta nuevamente";

                _logger.LogError(ex, "Error de conexión con NASA API en Rover");
                return View(new List<NasaPhoto>());
            }
            catch (JsonException ex)
            {
                // Manejo de errores en el formato de datos
                ViewData["ErrorMessage"] = "Error en el formato de datos";
                ViewData["ErrorDetails"] = "La NASA API devolvió datos en un formato inesperado";
                ViewData["ErrorResolution"] = "Por favor intenta nuevamente más tarde";

                _logger.LogError(ex, "Error de formato de datos de NASA API en Rover");
                return View(new List<NasaPhoto>());
            }
            catch (Exception ex)
            {
                // Manejo genérico para cualquier otro error
                ViewData["ErrorMessage"] = "Error inesperado";
                ViewData["ErrorDetails"] = $"Ocurrió un problema: {ex.Message}";
                ViewData["ErrorResolution"] = "Por favor intenta nuevamente o contacta al soporte técnico";

                _logger.LogError(ex, "Error inesperado en Rover");
                return View(new List<NasaPhoto>());
            }
        }
    }
}
