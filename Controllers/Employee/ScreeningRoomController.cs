using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]

    public class ScreeningRoomController : Controller
    {
        private readonly ScreeningRoomService _service;

        public ScreeningRoomController(ScreeningRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateScreeningRoomDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml", dto);

            var success = await _service.AddAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Sala została dodana pomyślnie.";
                return RedirectToAction("Index", "Employee");
            }

            ModelState.AddModelError("", "Wystąpił błąd podczas dodawania sali.");
            return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml", dto);
        }
    }
}
