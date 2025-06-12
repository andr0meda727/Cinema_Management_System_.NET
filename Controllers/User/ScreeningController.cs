using Cinema_Management_System.Models;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.User
{
    [Route("Screening")]
    public class ScreeningController : Controller
    {
        private readonly IScreeningService _screeningService;

        public ScreeningController(IScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var screenings = await _screeningService.GetScreeningsAsyncDate(selectedDate);

            var model = new ScreeningViewModel
            {
                SelectedDate = selectedDate,
                Screenings = screenings
            };

            return View(model);
        }

        [HttpGet]
        [Route("Details/{screeningId:int}")]
        public async Task<IActionResult> Details(int screeningId)
        {
            if (screeningId <= 0)
                return BadRequest("Invalid screening ID");

            var screening = await _screeningService.GetDetailedScreeningByIdAsync(screeningId);

            if (screening == null)
            {
                return NotFound($"Screening with ID {screeningId} not found");
            }

            return View(screening);
        }
    }
}
