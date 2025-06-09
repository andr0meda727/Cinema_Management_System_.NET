using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    [Route("EditMovie")]
    public class EditMovieController : Controller
    {
        private readonly EditMovieService _service;

        public EditMovieController(EditMovieService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie()
        {
            var movies = await _service.GetAllMoviesAsync();
            return View("~/Views/Employee/Movie/EditMovie.cshtml", movies);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> EditOneMovie(int id)
        {
            var movieDto = await _service.GetByIdAsync(id);
            if (movieDto == null)
            {
                return NotFound();
            }

            return View("~/Views/Employee/Movie/EditOneMovie.cshtml", movieDto);
        }

        // Obsługa formularza POST
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOneMovie(int id, EditMovieDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Employee/Movie/EditOneMovie.cshtml", dto);
            }

            var success = await _service.UpdateMovieAsync(id, dto);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction("EditMovie"); // np. powrót do listy filmów
        }
    }
}
