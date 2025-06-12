using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    [Route("EditMovie")]
    public class EditMovieController : Controller
    {
        private readonly IEditMovieService _service;
        private readonly ILogger<EditMovieController> _logger;

        public EditMovieController(IEditMovieService service, ILogger<EditMovieController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy filmów do edycji.");
                var movies = await _service.GetAllMoviesAsync();
                _logger.LogInformation("Pobrano {Count} filmów do edycji.", movies.Count);
                return View("~/Views/Employee/Movie/EditMovie.cshtml", movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania listy filmów do edycji.");
                TempData["ErrorMessage"] = "Nie udało się załadować listy filmów.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> EditOneMovie(int id)
        {
            try
            {
                _logger.LogInformation("Próba edycji filmu o ID: {Id}", id);
                var movieDto = await _service.GetByIdAsync(id);

                if (movieDto == null)
                {
                    _logger.LogWarning("Nie znaleziono filmu o ID: {Id}", id);
                    return NotFound();
                }

                ViewBag.HasScreenings = movieDto.HasScreenings;
                return View("~/Views/Employee/Movie/EditOneMovie.cshtml", movieDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas ładowania formularza edycji filmu o ID: {Id}", id);
                TempData["ErrorMessage"] = "Nie udało się załadować szczegółów filmu.";
                return RedirectToAction("EditMovie");
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOneMovie(int id, EditMovieDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Formularz edycji filmu o ID: {Id} zawiera błędy walidacji.", id);
                    ViewBag.HasScreenings = dto.HasScreenings;
                    return View("~/Views/Employee/Movie/EditOneMovie.cshtml", dto);
                }

                var success = await _service.UpdateMovieAsync(id, dto);

                if (!success)
                {
                    _logger.LogWarning("Nie można edytować filmu o ID: {Id} – istnieją zaplanowane seanse.", id);
                    ViewBag.HasScreenings = dto.HasScreenings;
                    ViewBag.ErrorMessage = "Nie można zmienić tytułu ani długości filmu, ponieważ istnieją już zaplanowane seanse.";
                    return View("~/Views/Employee/Movie/EditOneMovie.cshtml", dto);
                }

                _logger.LogInformation("Pomyślnie zaktualizowano film o ID: {Id}", id);
                return RedirectToAction("EditMovie");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas edycji filmu o ID: {Id}", id);
                TempData["ErrorMessage"] = "Wystąpił błąd podczas zapisywania zmian.";
                return RedirectToAction("EditMovie");
            }
        }
    }
}
