using Cinema_Management_System.DTOs.Employee;
using Microsoft.AspNetCore.Mvc;

[Route("EditScreenings")]
public class EditScreeningController : Controller
{
    private readonly EditScreeningService _service;

    public EditScreeningController(EditScreeningService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var screenings = await _service.GetAllAsync();
        return View("~/Views/Employee/Screening/EditScreenings.cshtml", screenings);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            TempData["ErrorMessage"] = "Nie można edytować seansu (bilety sprzedane lub nie istnieje).";
            return RedirectToAction("Index");
        }

        return View("~/Views/Employee/Screening/EditOneScreening.cshtml", dto);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditScreeningDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Employee/Screening/EditOneScreening.cshtml", dto);
        }

        var success = await _service.UpdateAsync(dto);
        if (!success)
        {
            ViewBag.ErrorMessage = "Nie można zapisać zmian. Sprawdź konflikt z innymi seansami lub bilety.";
            return View("~/Views/Employee/Screening/EditOneScreening.cshtml", dto);
        }

        return RedirectToAction("Index");
    }
}
