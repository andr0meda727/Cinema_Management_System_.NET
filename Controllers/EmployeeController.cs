using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // widok: Views/Employee/Index.cshtml
        }

        public IActionResult AddMovie()
        {
            return View();
        }

        public IActionResult EditMovies()
        {
            return View();
        }

        public IActionResult DeleteMovie()
        {
            return View();
        }

        public IActionResult BrowseMovies()
        {
            return View();
        }

        public IActionResult AddScreeningRoom()
        {
            return View();
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
