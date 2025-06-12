using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    public class DeleteScreeningRoomController : Controller
    {
        private readonly IDeleteScreeningRoomService _service;
        private readonly ILogger<DeleteScreeningRoomController> _logger;

        public DeleteScreeningRoomController(IDeleteScreeningRoomService service, ILogger<DeleteScreeningRoomController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteScreeningRoom()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy sal do usunięcia.");
                var screeningRooms = await _service.GetAllAsync();
                _logger.LogInformation("Pobrano {Count} sal do wyświetlenia.", screeningRooms.Count);
                return View("~/Views/Employee/ScreeningRoom/DeleteScreeningRoom.cshtml", screeningRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas ładowania widoku usuwania sal kinowych.");
                TempData["ErrorMessage"] = "Nie udało się załadować listy sal.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScreeningRoom(List<int> selectedRoomIds)
        {
            try
            {
                if (selectedRoomIds == null || !selectedRoomIds.Any())
                {
                    _logger.LogWarning("Nie wybrano żadnych sal do usunięcia.");
                    TempData["ErrorMessage"] = "Nie wybrano żadnych sal do usunięcia.";
                    return RedirectToAction("DeleteScreeningRoom");
                }

                _logger.LogInformation("Próba usunięcia {Count} sal: {Ids}", selectedRoomIds.Count, string.Join(", ", selectedRoomIds));
                var (deleted, blocked) = await _service.DeleteAsync(selectedRoomIds);

                if (deleted.Any())
                {
                    _logger.LogInformation("Usunięto sale: {Ids}", string.Join(", ", deleted));
                    TempData["SuccessMessage"] = $"Usunięto {deleted.Count} sal(e) kinowe.";
                }

                if (blocked.Any())
                {
                    _logger.LogWarning("Nie można usunąć sal (mają zaplanowane seanse): {Ids}", string.Join(", ", blocked));
                    TempData["ErrorMessage"] = $"Nie można usunąć {blocked.Count} sal – mają zaplanowane seanse.";
                }

                return RedirectToAction("DeleteScreeningRoom");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas usuwania sal.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania sal.";
                return RedirectToAction("DeleteScreeningRoom");
            }
        }
    }
}
