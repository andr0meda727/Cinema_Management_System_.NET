using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    [Route("EditMovie")]
    public class EditMovieController : Controller
    {
        private readonly IEditMovieService _service;

        public EditMovieController(IEditMovieService service)
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
            ViewBag.HasScreenings = movieDto.HasScreenings;
            return View("~/Views/Employee/Movie/EditOneMovie.cshtml", movieDto);
        }

        // Obsługa formularza POST
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOneMovie(int id, EditMovieDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HasScreenings = dto.HasScreenings;
                return View("~/Views/Employee/Movie/EditOneMovie.cshtml", dto);
            }

            var success = await _service.UpdateMovieAsync(id, dto);
            if (!success)
            {
                ViewBag.HasScreenings = dto.HasScreenings;
                ViewBag.ErrorMessage = "Nie można zmienić tytułu ani długości filmu, ponieważ istnieją już zaplanowane seanse.";
                return View("~/Views/Employee/Movie/EditOneMovie.cshtml", dto);
            }

            // SUKCES – wróć do listy
            return RedirectToAction("EditMovie");
        }
    }
}
