using Cinema_Management_System.Models;
using Cinema_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    [Route("Screening")]
    public class ScreeningController : Controller
    {
        private readonly ScreeningService _screeningService;

        public ScreeningController(ScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
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
            var screening = await _screeningService.GetDetailedScreeningByIdAsync(screeningId);

            if (screening == null)
            {
                return NotFound();
            }

            return View(screening);
        }
    }
}
