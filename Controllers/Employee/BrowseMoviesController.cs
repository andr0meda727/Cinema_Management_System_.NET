using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class BrowseMoviesController : Controller
    {
        private readonly IBrowseMoviesService _service;
        private readonly ILogger<BrowseMoviesController> _logger;

        public BrowseMoviesController(IBrowseMoviesService service, ILogger<BrowseMoviesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> BrowseMovies()
        {
            try
            {
                _logger.LogInformation("Rozpoczęto pobieranie listy filmów dla pracownika.");
                var movies = await _service.GetAllMoviesAsync();
                _logger.LogInformation("Pomyślnie pobrano {Count} filmów.", movies.Count);
                return View("~/Views/Employee/Movie/BrowseMovies.cshtml", movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas pobierania listy filmów.");
                TempData["ErrorMessage"] = "Nie udało się pobrać listy filmów.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
