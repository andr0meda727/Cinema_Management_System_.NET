using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class DeleteMovieController : Controller
    {
        private readonly IDeleteMovieService _service;

        public DeleteMovieController(IDeleteMovieService service)
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
            if (selectedMovieIds == null || !selectedMovieIds.Any())
            {
                TempData["ErrorMessage"] = "Nie wybrano żadnych filmów do usunięcia.";
                return RedirectToAction("DeleteMovie");
            }

            var (deleted, blocked) = await _service.DeleteAsync(selectedMovieIds);

            if (deleted.Any())
            {
                TempData["SuccessMessage"] = $"Usunięto {deleted.Count} film(y).";
            }

            if (blocked.Any())
            {
                TempData["ErrorMessage"] = $"Nie można usunąć {blocked.Count} filmów – mają zaplanowane seanse.";
            }

            return RedirectToAction("DeleteMovie");
        }
    }
}
