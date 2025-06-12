using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly IDeleteMovieService _deleteMovieService;
        private readonly IDeleteScreeningRoomService _deleteScreeningRoomService;
        private readonly IEditMovieService _editMovieService;
        private readonly IBrowseScreeningRoomService _browseScreeningRoomService;


        public EmployeeController(IDeleteMovieService deleteMovieService, 
            IDeleteScreeningRoomService deleteScreeningRoomService,
            IEditMovieService editMovieService,
            IBrowseScreeningRoomService browseScreeningRoomService)
        {
            _deleteMovieService = deleteMovieService;
            _deleteScreeningRoomService = deleteScreeningRoomService;
            _editMovieService = editMovieService;
            _browseScreeningRoomService = browseScreeningRoomService;
        }
        public IActionResult Index()
        {
            return View(); // widok: Views/Employee/Index.cshtml
        }

        public IActionResult AddMovie()
        {
            return View("~/Views/Employee/Movie/AddMovie.cshtml");
        }

        public IActionResult EditMovie()
        {
            return View("~/Views/Employee/Movie/EditMovie.cshtml");
        }

        public async Task<IActionResult> DeleteMovie()
        {
            var movies = await _deleteMovieService.GetAllAsync();
            return View("~/Views/Employee/Movie/DeleteMovie.cshtml", movies);
        }

        public IActionResult BrowseMovies()
        {
            return View("~/Views/Employee/Movie/BrowseMovies.cshtml");
        }

        public IActionResult AddScreeningRoom()
        {
            return View("~/Views/Employee/ScreeningRoom/AddScreeningRoom.cshtml");
        }

        public IActionResult EditScreeningRooms()
        {
            return View();
        }

        public async Task<IActionResult> DeleteScreeningRoom()
        {
            var screeningRooms = await _deleteScreeningRoomService.GetAllAsync();
            return View("~/Views/Employee/ScreeningRoom/DeleteScreeningRoom.cshtml", screeningRooms);
        }
        public async Task<IActionResult> BrowseScreeningRooms()
        {
            var rooms = await _browseScreeningRoomService.GetAllAsync();
            return View("~/Views/Employee/ScreeningRoom/BrowseScreeningRooms.cshtml", rooms);
        }

        public async Task<IActionResult> ViewRoomDetails(int id)
        {
            var room = await _browseScreeningRoomService.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View("~/Views/Employee/ScreeningRoom/ViewRoomDetails.cshtml", room);
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
