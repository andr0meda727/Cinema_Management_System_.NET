using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    public class DeleteScreeningRoomController : Controller
    {
        private readonly IDeleteScreeningRoomService _service;

        public DeleteScreeningRoomController(IDeleteScreeningRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteScreeningRoom()
        {
            var screeningRooms = await _service.GetAllAsync();

            return View("~/Views/Employee/ScreeningRoom/DeleteScreeningRoom.cshtml", screeningRooms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScreeningRoom(List<int> selectedRoomIds)
        {
            if (selectedRoomIds == null || !selectedRoomIds.Any())
            {
                TempData["ErrorMessage"] = "Nie wybrano żadnych sal do usunięcia.";
                return RedirectToAction("DeleteScreeningRoom");
            }

            var (deleted, blocked) = await _service.DeleteAsync(selectedRoomIds);

            if (deleted.Any())
            {
                TempData["SuccessMessage"] = $"Usunięto {deleted.Count} sal(e) kinowe.";
            }

            if (blocked.Any())
            {
                TempData["ErrorMessage"] = $"Nie można usunąć {blocked.Count} sal – mają zaplanowane seanse.";
            }

            return RedirectToAction("DeleteScreeningRoom");
        }
    }
}
