using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class AddMovieController : Controller
    {
        private readonly AddMovieService _service;

        public AddMovieController(AddMovieService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("~/Views/Employee/Movie/AddMovie.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMovieDTO dto)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Employee/Movie/AddMovie.cshtml", dto);

            var success = await _service.AddAsync(dto);
            if (success)
            {
                TempData["SuccessMessage"] = "Film został dodany.";
                return View("~/Views/Employee/Movie/AddMovie.cshtml");
            }
            else {
                TempData["ErrorMessage"] = "Film o podanym tytule już istnieje.";
            }

            return View("~/Views/Employee/Movie/AddMovie.cshtml", dto);
        }
    }
}

