using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class BrowseScreeningController : Controller
    {
        private readonly IBrowseScreeningService _service;
        private readonly ILogger<BrowseScreeningController> _logger;

        public BrowseScreeningController(IBrowseScreeningService service, ILogger<BrowseScreeningController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            try
            {
                DateTime selectedDate = date ?? DateTime.Today;
                _logger.LogInformation("Pobieranie seansów dla daty: {Date}", selectedDate.ToShortDateString());

                var screenings = await _service.GetScreeningsByDateAsync(selectedDate);
                _logger.LogInformation("Pobrano {Count} seansów dla daty: {Date}", screenings.Count, selectedDate.ToShortDateString());

                ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
                return View("~/Views/Employee/Screening/BrowseScreenings.cshtml", screenings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas pobierania seansów.");
                TempData["ErrorMessage"] = "Nie udało się pobrać listy seansów.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
