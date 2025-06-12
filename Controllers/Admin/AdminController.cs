using Cinema_Management_System.DTOs.Auth;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(IAuthService authService,
                    UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> RegisterEmployee(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.RegisterEmployeeAsync(model);
            if (result.Succeeded)
                return RedirectToAction("RegisterEmployee", "Admin");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

    }
}
