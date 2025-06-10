using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    public class DeleteScreeningController : Controller
    {
        private readonly DeleteScreeningService _service;

        public DeleteScreeningController(DeleteScreeningService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteScreening()
        {
            var screenings = await _service.GetAllAsync();
            return View("~/Views/Employee/Screening/DeleteScreening.cshtml", screenings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScreening(List<int> selectedIds)
        {
            var (deleted, blocked) = await _service.DeleteAsync(selectedIds);

            if (deleted.Any())
                TempData["SuccessMessage"] = $"Usunięto {deleted.Count} seans(ów).";

            if (blocked.Any())
                TempData["ErrorMessage"] = $"Nie można usunąć {blocked.Count} seansów, ponieważ mają przypisane bilety.";

            return RedirectToAction("DeleteScreening");
        }
    }
}
