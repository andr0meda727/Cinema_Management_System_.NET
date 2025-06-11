using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class AddScreeningController : Controller
    {
        private readonly AddScreeningService _service;

        public AddScreeningController(AddScreeningService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> AddScreening()
        {
            ViewBag.Movies = await _service.GetMoviesAsync();
            ViewBag.Rooms = await _service.GetRoomsAsync();
            return View("~/Views/Employee/Screening/AddScreening.cshtml", new AddScreeningDTO());
        }

        [HttpPost]
        public async Task<IActionResult> AddScreening(AddScreeningDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Movies = await _service.GetMoviesAsync();
                ViewBag.Rooms = await _service.GetRoomsAsync();
                return View("~/Views/Employee/Screening/AddScreening.cshtml", dto);
            }

            var (success, msg) = await _service.AddAsync(dto);
            if (success)
            {
                TempData["SuccessMessage"] = "Pomyślnie dodano seans.";
                return RedirectToAction("AddScreening");
            }
            else {
                TempData["ErrorMessage"] =  msg;
                //ModelState.AddModelError("", msg ?? "Unknown error.");
            }

            
            ViewBag.Movies = await _service.GetMoviesAsync();
            ViewBag.Rooms = await _service.GetRoomsAsync();
            return View("~/Views/Employee/Screening/AddScreening.cshtml", dto);
        }
    }
}
