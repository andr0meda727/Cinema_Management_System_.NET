using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class DeleteMovieController : Controller
    {
        private readonly DeleteMovieService _service;

        public DeleteMovieController(DeleteMovieService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMovie()
        {
            var movies = await _service.GetAllAsync();
            return View("~/Views/Employee/Movie/DeleteMovie.cshtml", movies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(List<int> selectedMovieIds)
        {
            if (selectedMovieIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Nie wybrano żadnego filmu do usunięcia.";
                return RedirectToAction(nameof(DeleteMovie));
            }

            await _service.DeleteAsync(selectedMovieIds);
            TempData["SuccessMessage"] = "Wybrane filmy zostały usunięte.";
            return RedirectToAction(nameof(DeleteMovie));
        }
    }
}
