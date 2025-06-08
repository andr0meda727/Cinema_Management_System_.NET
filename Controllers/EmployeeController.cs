using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly DeleteMovieService _deleteMovieService;

        public EmployeeController(DeleteMovieService deleteMovieService)
        {
            _deleteMovieService = deleteMovieService;
        }
        public IActionResult Index()
        {
            return View(); // widok: Views/Employee/Index.cshtml
        }

        public IActionResult AddMovie()
        {
            return View("~/Views/Employee/Movie/AddMovie.cshtml");
        }

        public IActionResult EditMovies()
        {
            return View();
        }

        public async Task<IActionResult> DeleteMovie()
        {
            var movies = await _deleteMovieService.GetAllAsync();
            return View("~/Views/Employee/Movie/DeleteMovie.cshtml", movies);
        }

        public IActionResult BrowseMovies()
        {
            return View();
        }

        public IActionResult AddScreeningRoom()
        {
            return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml");
        }

        public IActionResult EditScreeningRooms()
        {
            return View();
        }

        public IActionResult DeleteScreeningRoom()
        {
            return View();
        }

        public IActionResult BrowseScreeningRooms()
        {
            return View();
        }

        public IActionResult AddScreening()
        {
            return View();
        }

        public IActionResult EditScreenings()
        {
            return View();
        }

        public IActionResult DeleteScreening()
        {
            return View();
        }

        public IActionResult BrowseScreenings()
        {
            return View();
        }

        public IActionResult TicketSalesReport()
        {
            return View();
        }
    }
}
