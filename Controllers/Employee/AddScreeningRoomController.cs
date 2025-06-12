using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class AddScreeningRoomController : Controller
    {
        private readonly IAddScreeningRoomService _service;
        private readonly ILogger<AddScreeningRoomController> _logger;

        public AddScreeningRoomController(IAddScreeningRoomService service, ILogger<AddScreeningRoomController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Add()
        {
            _logger.LogInformation("Wyświetlanie formularza dodawania sali kinowej.");
            return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateScreeningRoomDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Formularz dodawania sali zawiera nieprawidłowe dane.");
                    return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml", dto);
                }

                var success = await _service.AddAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Sala '{Name}' została dodana pomyślnie.", dto.Name);
                    TempData["SuccessMessage"] = "Sala została dodana pomyślnie.";
                    return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml");
                }
                else
                {
                    _logger.LogWarning("Nie udało się dodać sali '{Name}' – prawdopodobnie już istnieje.", dto.Name);
                    TempData["ErrorMessage"] = "Wystąpił błąd podczas dodawania sali. Sprawdź czy nazwa nie koliduje z istniejącą";
                    return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml", dto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił wyjątek podczas dodawania sali kinowej.");
                TempData["ErrorMessage"] = "Wystąpił błąd wewnętrzny podczas dodawania sali.";
                return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml", dto);
            }
        }
    }
}
