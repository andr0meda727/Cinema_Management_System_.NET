using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class BrowseScreeningRoomController : Controller
    {
        private readonly IBrowseScreeningRoomService _service;

        public BrowseScreeningRoomController(IBrowseScreeningRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rooms = await _service.GetAllAsync();
            return View("~/Views/Employee/ScreeningRoom/BrowseScreeningRooms.cshtml", rooms);
        }

        [HttpGet("Employee/ScreeningRoom/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var room = await _service.GetByIdAsync(id);
            if (room == null) return NotFound();

            return View("~/Views/Employee/ScreeningRoom/ScreeningRoomDetails.cshtml", room);
        }
    }
}
