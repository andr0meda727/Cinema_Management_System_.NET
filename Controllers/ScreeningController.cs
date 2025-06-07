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
        public async Task<IActionResult> Screening(DateTime? date)
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
    }
}
