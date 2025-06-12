using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[Route("EditScreenings")]
public class EditScreeningController : Controller
{
    private readonly IEditScreeningService _service;
    private readonly ILogger<EditScreeningController> _logger;

    public EditScreeningController(IEditScreeningService service, ILogger<EditScreeningController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            _logger.LogInformation("Pobieranie listy seansów do edycji.");
            var screenings = await _service.GetAllAsync();
            _logger.LogInformation("Pobrano {Count} seansów do edycji.", screenings.Count);
            return View("~/Views/Employee/Screening/EditScreenings.cshtml", screenings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania listy seansów.");
            TempData["ErrorMessage"] = "Nie udało się załadować listy seansów.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            _logger.LogInformation("Próba edycji seansu o ID: {Id}", id);
            var dto = await _service.GetByIdAsync(id);

            if (dto == null)
            {
                _logger.LogWarning("Nie znaleziono seansu do edycji lub bilety już zostały sprzedane (ID: {Id})", id);
                TempData["ErrorMessage"] = "Nie można edytować seansu (bilety sprzedane lub nie istnieje).";
                return RedirectToAction("Index");
            }

            return View("~/Views/Employee/Screening/EditOneScreening.cshtml", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas ładowania formularza edycji seansu o ID: {Id}", id);
            TempData["ErrorMessage"] = "Wystąpił błąd podczas ładowania seansu.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditScreeningDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Formularz edycji seansu o ID: {Id} zawiera nieprawidłowe dane.", id);
                return View("~/Views/Employee/Screening/EditOneScreening.cshtml", dto);
            }

            var success = await _service.UpdateAsync(dto);

            if (!success)
            {
                _logger.LogWarning("Nie udało się zapisać zmian dla seansu o ID: {Id} – konflikt terminów lub bilety już sprzedane.", id);
                ViewBag.ErrorMessage = "Nie można zapisać zmian. Sprawdź konflikt z innymi seansami lub bilety.";
                return View("~/Views/Employee/Screening/EditOneScreening.cshtml", dto);
            }

            _logger.LogInformation("Pomyślnie zaktualizowano seans o ID: {Id}", id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas zapisywania edycji seansu o ID: {Id}", id);
            TempData["ErrorMessage"] = "Wystąpił błąd podczas zapisywania zmian.";
            return RedirectToAction("Index");
        }
    }
}
