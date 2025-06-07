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
        public async Task<IActionResult> Screening()
        {
            var screenings = await _screeningService.GetUpcomingScreeningsAsync();
            return View(screenings);
        }
    }
}
