using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class EditScreeningRoomController : Controller
    {
        private readonly EditScreeningRoomService _editService;

        public EditScreeningRoomController(EditScreeningRoomService editService)
        {
            _editService = editService;
        }

        [HttpGet]
        public async Task<IActionResult> EditScreeningRooms()
        {
            var rooms = await _editService.GetAllAsync();
            return View("~/Views/Employee/ScreeningRoom/EditScreeningRooms.cshtml", rooms);
        }

        [HttpPost]
        public async Task<IActionResult> EditScreeningRoom(EditScreeningRoomDTO dto)
        {
            var success = await _editService.UpdateAsync(dto);

            if (success)
                TempData["SuccessMessage"] = "Zmiany zapisane pomyślnie.";
            else
                TempData["ErrorMessage"] = "Nie można edytować sali z przyszłymi seansami.";

            return RedirectToAction(nameof(EditScreeningRooms));
        }
    }
}
