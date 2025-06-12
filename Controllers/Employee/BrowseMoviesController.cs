using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class BrowseMoviesController : Controller
    {
        private readonly IBrowseMoviesService _service;

        public BrowseMoviesController(IBrowseMoviesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> BrowseMovies()
        {
            var movies = await _service.GetAllMoviesAsync();
            return View("~/Views/Employee/Movie/BrowseMovies.cshtml", movies);
        }
    }
}
