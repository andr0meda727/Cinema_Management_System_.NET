using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class BrowseScreeningController : Controller
    {
        private readonly IBrowseScreeningService _service;

        public BrowseScreeningController(IBrowseScreeningService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            DateTime selectedDate = date ?? DateTime.Today;
            var screenings = await _service.GetScreeningsByDateAsync(selectedDate);

            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");

            return View("~/Views/Employee/Screening/BrowseScreenings.cshtml", screenings);
        }
    }
}
