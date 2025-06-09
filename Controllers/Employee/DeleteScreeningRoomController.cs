using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    public class DeleteScreeningRoomController : Controller
    {
        private readonly DeleteScreeningRoomService _service;

        public DeleteScreeningRoomController(DeleteScreeningRoomService service)
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
        public async Task<IActionResult> DeleteScreeningRoom(List<int> selectedScreeningRoomIds)
        {
            if (selectedScreeningRoomIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Nie wybrano sali do usunięcia.";
                return RedirectToAction(nameof(DeleteScreeningRoom));
            }

            await _service.DeleteAsync(selectedScreeningRoomIds);
            TempData["SuccessMessage"] = "Wybrane sale zostały usunięte.";
            return RedirectToAction(nameof(DeleteScreeningRoom));
        }
    }
}
