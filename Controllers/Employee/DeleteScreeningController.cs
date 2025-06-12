using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class DeleteScreeningController : Controller
    {
        private readonly IDeleteScreeningService _service;
        private readonly ILogger<DeleteScreeningController> _logger;

        public DeleteScreeningController(IDeleteScreeningService service, ILogger<DeleteScreeningController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteScreening()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy seansów do usunięcia.");
                var screenings = await _service.GetAllAsync();
                _logger.LogInformation("Pobrano {Count} seansów do wyświetlenia.", screenings.Count);
                return View("~/Views/Employee/Screening/DeleteScreening.cshtml", screenings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania listy seansów.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas ładowania listy seansów.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScreening(List<int> selectedIds)
        {
            try
            {
                if (selectedIds == null || !selectedIds.Any())
                {
                    _logger.LogWarning("Próba usunięcia seansów bez wybranych ID.");
                    TempData["ErrorMessage"] = "Nie wybrano żadnych seansów do usunięcia.";
                    return RedirectToAction("DeleteScreening");
                }

                _logger.LogInformation("Próba usunięcia {Count} seansów: {Ids}", selectedIds.Count, string.Join(", ", selectedIds));
                var (deleted, blocked) = await _service.DeleteAsync(selectedIds);

                if (deleted.Any())
                {
                    _logger.LogInformation("Usunięto seanse: {Ids}", string.Join(", ", deleted));
                    TempData["SuccessMessage"] = $"Usunięto {deleted.Count} seans(ów).";
                }

                if (blocked.Any())
                {
                    _logger.LogWarning("Zablokowano usunięcie seansów (posiadają bilety): {Ids}", string.Join(", ", blocked));
                    TempData["ErrorMessage"] = $"Nie można usunąć {blocked.Count} seansów, ponieważ mają przypisane bilety.";
                }

                return RedirectToAction("DeleteScreening");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas usuwania seansów.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania seansów.";
                return RedirectToAction("DeleteScreening");
            }
        }
    }
}
