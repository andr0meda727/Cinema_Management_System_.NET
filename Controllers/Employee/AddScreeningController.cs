using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class AddScreeningController : Controller
    {
        private readonly IAddScreeningService _service;
        private readonly ILogger<AddScreeningController> _logger;

        public AddScreeningController(IAddScreeningService service, ILogger<AddScreeningController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> AddScreening()
        {
            try
            {
                _logger.LogInformation("Loading AddScreening form.");
                ViewBag.Movies = await _service.GetMoviesAsync();
                ViewBag.Rooms = await _service.GetRoomsAsync();
                return View("~/Views/Employee/Screening/AddScreening.cshtml", new AddScreeningDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load AddScreening form.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas ładowania formularza.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddScreening(AddScreeningDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Submitted AddScreening form is invalid.");
                    ViewBag.Movies = await _service.GetMoviesAsync();
                    ViewBag.Rooms = await _service.GetRoomsAsync();
                    return View("~/Views/Employee/Screening/AddScreening.cshtml", dto);
                }

                var (success, msg) = await _service.AddAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Successfully added screening.");
                    TempData["SuccessMessage"] = "Pomyślnie dodano seans.";
                    return RedirectToAction("AddScreening");
                }
                else
                {
                    _logger.LogWarning("Failed to add screening: {Message}", msg);
                    TempData["ErrorMessage"] = msg;
                }

                ViewBag.Movies = await _service.GetMoviesAsync();
                ViewBag.Rooms = await _service.GetRoomsAsync();
                return View("~/Views/Employee/Screening/AddScreening.cshtml", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding screening.");
                TempData["ErrorMessage"] = "Wystąpił nieoczekiwany błąd podczas dodawania seansu.";
                return RedirectToAction("AddScreening");
            }
        }
    }
}
