using System.Security.Claims;
using Cinema_Management_System.DTOs.Auth;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Auth;
using Cinema_Management_System.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{

    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(IAuthService authService,
                    UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Views/Auth/Login.cshtml
        }
        
        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var token = await _authService.LoginAsync(model);

            if (token == null)
                return Unauthorized("Invalid username or password.");

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(10)
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var result = await _authService.RegisterUserAsync(model);
            if (result.Succeeded)
                return RedirectToAction("Login", "Auth");

            return BadRequest(result.Errors);
        }
       
    }
}
