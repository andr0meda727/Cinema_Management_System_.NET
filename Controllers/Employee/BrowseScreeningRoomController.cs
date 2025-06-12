using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class BrowseScreeningRoomController : Controller
    {
        private readonly IBrowseScreeningRoomService _service;
        private readonly ILogger<BrowseScreeningRoomController> _logger;

        public BrowseScreeningRoomController(IBrowseScreeningRoomService service, ILogger<BrowseScreeningRoomController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Rozpoczęto pobieranie listy sal kinowych.");
                var rooms = await _service.GetAllAsync();
                _logger.LogInformation("Pobrano {Count} sal kinowych.", rooms.Count);
                return View("~/Views/Employee/ScreeningRoom/BrowseScreeningRooms.cshtml", rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania sal kinowych.");
                TempData["ErrorMessage"] = "Nie udało się pobrać listy sal kinowych.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("Employee/ScreeningRoom/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation("Pobieranie szczegółów sali o ID: {Id}", id);
                var room = await _service.GetByIdAsync(id);

                if (room == null)
                {
                    _logger.LogWarning("Nie znaleziono sali o ID: {Id}", id);
                    return NotFound();
                }

                return View("~/Views/Employee/ScreeningRoom/ScreeningRoomDetails.cshtml", room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania szczegółów sali o ID: {Id}", id);
                TempData["ErrorMessage"] = "Nie udało się pobrać szczegółów sali.";
                return RedirectToAction("Index");
            }
        }
    }
}
