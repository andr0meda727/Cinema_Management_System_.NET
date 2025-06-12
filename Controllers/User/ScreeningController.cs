using Cinema_Management_System.Models;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.User
{
    [Route("Screening")]
    public class ScreeningController : Controller
    {
        private readonly IScreeningService _screeningService;
        private readonly ILogger<ScreeningController> _logger;

        public ScreeningController(IScreeningService screeningService, ILogger<ScreeningController> logger)
        {
            _screeningService = screeningService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            _logger.LogInformation("Fetching screenings for date: {SelectedDate}", selectedDate);
            var screenings = await _screeningService.GetScreeningsAsyncDate(selectedDate);

            var model = new ScreeningViewModel
            {
                SelectedDate = selectedDate,
                Screenings = screenings
            };

            _logger.LogInformation("Found {ScreeningCount} screenings for date {SelectedDate}", screenings.Count, selectedDate);

            return View(model);
        }

        [HttpGet]
        [Route("Details/{screeningId:int}")]
        public async Task<IActionResult> Details(int screeningId)
        {
            if (screeningId <= 0)
            {
                _logger.LogWarning("Invalid screening ID: {ScreeningId}", screeningId);
                return BadRequest("Invalid screening ID");
            }

            var screening = await _screeningService.GetDetailedScreeningByIdAsync(screeningId);

            if (screening == null)
            {
                _logger.LogWarning("Screening not found for ID: {ScreeningId}", screeningId);
                return NotFound($"Screening with ID {screeningId} not found");
            }

            _logger.LogInformation("Successfully retrieved screening details for ID: {ScreeningId}", screeningId);
            return View(screening);
        }
    }
}
