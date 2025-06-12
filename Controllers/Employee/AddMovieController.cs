using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class AddMovieController : Controller
    {
        private readonly IAddMovieService _service;
        private readonly ILogger<AddMovieController> _logger;

        public AddMovieController(IAddMovieService service, ILogger<AddMovieController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Add()
        {
            _logger.LogInformation("Wyświetlanie formularza dodawania filmu.");
            return View("~/Views/Employee/Movie/AddMovie.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMovieDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Formularz dodawania filmu zawiera nieprawidłowe dane.");
                    return View("~/Views/Employee/Movie/AddMovie.cshtml", dto);
                }

                var success = await _service.AddAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Film '{Title}' został pomyślnie dodany.", dto.Title);
                    TempData["SuccessMessage"] = "Film został dodany.";
                    return View("~/Views/Employee/Movie/AddMovie.cshtml");
                }
                else
                {
                    _logger.LogWarning("Próba dodania filmu zakończona niepowodzeniem – tytuł już istnieje: {Title}", dto.Title);
                    TempData["ErrorMessage"] = "Film o podanym tytule już istnieje.";
                    return View("~/Views/Employee/Movie/AddMovie.cshtml", dto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas dodawania filmu.");
                TempData["ErrorMessage"] = "Wystąpił błąd podczas dodawania filmu.";
                return View("~/Views/Employee/Movie/AddMovie.cshtml", dto);
            }
        }
    }
}
