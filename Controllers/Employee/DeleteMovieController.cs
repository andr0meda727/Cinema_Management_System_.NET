using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class DeleteMovieController : Controller
    {
        private readonly IDeleteMovieService _service;
        private readonly ILogger<DeleteMovieController> _logger;

        public DeleteMovieController(IDeleteMovieService service, ILogger<DeleteMovieController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMovie()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy filmów do usunięcia.");
                var movies = await _service.GetAllAsync();
                _logger.LogInformation("Pobrano {Count} filmów do wyświetlenia.", movies.Count);
                return View("~/Views/Employee/Movie/DeleteMovie.cshtml", movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas ładowania widoku usuwania filmów.");
                TempData["ErrorMessage"] = "Nie udało się załadować listy filmów.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(List<int> selectedMovieIds)
        {
            try
            {
                if (selectedMovieIds == null || !selectedMovieIds.Any())
                {
                    _logger.LogWarning("Próba usunięcia filmów bez zaznaczenia żadnego ID.");
                    TempData["ErrorMessage"] = "Nie wybrano żadnych filmów do usunięcia.";
                    return RedirectToAction("DeleteMovie");
                }

                _logger.LogInformation("Rozpoczęto usuwanie {Count} filmów: {Ids}", selectedMovieIds.Count, string.Join(", ", selectedMovieIds));
                var (deleted, blocked) = await _service.DeleteAsync(selectedMovieIds);

                if (deleted.Any())
                {
                    _logger.LogInformation("Usunięto filmy: {Ids}", string.Join(", ", deleted));
                    TempData["SuccessMessage"] = $"Usunięto {deleted.Count} film(y).";
                }

                if (blocked.Any())
                {
                    _logger.LogWarning("Nie można usunąć filmów (zaplanowane seanse): {Ids}", string.Join(", ", blocked));
                    TempData["ErrorMessage"] = $"Nie można usunąć {blocked.Count} filmów – mają zaplanowane seanse.";
                }

                return RedirectToAction("DeleteMovie");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas usuwania filmów.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania filmów.";
                return RedirectToAction("DeleteMovie");
            }
        }
    }
}
