using Cinema_Management_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]

    public class DetailsController : Controller
    {
        private readonly CinemaDbContext _db;

        public DetailsController(CinemaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            return View("~/Views/Employee/Movie/Details.cshtml", movie);
        }
    }
}
