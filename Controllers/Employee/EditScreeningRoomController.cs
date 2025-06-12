using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class EditScreeningRoomController : Controller
    {
        private readonly IEditScreeningRoomService _editService;
        private readonly ILogger<EditScreeningRoomController> _logger;

        public EditScreeningRoomController(IEditScreeningRoomService editService, ILogger<EditScreeningRoomController> logger)
        {
            _editService = editService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> EditScreeningRooms()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy sal do edycji.");
                var rooms = await _editService.GetAllAsync();
                _logger.LogInformation("Pobrano {Count} sal kinowych do edycji.", rooms.Count);
                return View("~/Views/Employee/ScreeningRoom/EditScreeningRooms.cshtml", rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania listy sal kinowych.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas ładowania sal.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditScreeningRoom(EditScreeningRoomDTO dto)
        {
            try
            {
                _logger.LogInformation("Próba edycji sali o ID: {Id}", dto.Id);
                var success = await _editService.UpdateAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Pomyślnie edytowano salę o ID: {Id}", dto.Id);
                    TempData["SuccessMessage"] = "Zmiany zapisane pomyślnie.";
                }
                else
                {
                    _logger.LogWarning("Nie można edytować sali o ID: {Id} – ma zaplanowane przyszłe seanse.", dto.Id);
                    TempData["ErrorMessage"] = "Nie można edytować sali z przyszłymi seansami.";
                }

                return RedirectToAction(nameof(EditScreeningRooms));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas edycji sali o ID: {Id}", dto.Id);
                TempData["ErrorMessage"] = "Wystąpił błąd podczas zapisywania zmian sali.";
                return RedirectToAction(nameof(EditScreeningRooms));
            }
        }
    }
}
