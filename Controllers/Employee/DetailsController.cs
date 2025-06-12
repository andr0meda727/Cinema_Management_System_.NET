using Cinema_Management_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class DetailsController : Controller
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<DetailsController> _logger;

        public DetailsController(CinemaDbContext db, ILogger<DetailsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation("Próba pobrania szczegółów filmu o ID: {Id}", id);
                var movie = await _db.Movies.FindAsync(id);

                if (movie == null)
                {
                    _logger.LogWarning("Nie znaleziono filmu o ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Pomyślnie pobrano szczegóły filmu o ID: {Id}", id);
                return View("~/Views/Employee/Movie/Details.cshtml", movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania szczegółów filmu o ID: {Id}", id);
                TempData["ErrorMessage"] = "Wystąpił błąd podczas ładowania szczegółów filmu.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
