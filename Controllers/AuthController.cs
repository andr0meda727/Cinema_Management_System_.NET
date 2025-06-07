using Cinema_Management_System.DTOs.Auth;
using Cinema_Management_System.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

            return Ok(new { Token = token });
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
            var result = await _authService.RegisterAsync(model);
            if (result.Succeeded)
                return Ok("User registered successfully.");

            return BadRequest(result.Errors);
        }
       
    }
}
