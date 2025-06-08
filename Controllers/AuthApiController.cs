using System.Security.Claims;
using Cinema_Management_System.DTOs.Auth;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.Api
{
    //FOR POSTMAN TESTING
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICookieService _cookieService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthApiController(IAuthService authService,
             ICookieService cookieService, 
             UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _cookieService = cookieService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var token = await _authService.LoginAsync(model);

            if (token == null)
                return Unauthorized("Invalid username or password.");

            _cookieService.SetTokenCookie(Response, token);

            return Ok(new { message = "Login successful" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _cookieService.DeleteTokenCookie(Response);
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var result = await _authService.RegisterUserAsync(model);
            if (result.Succeeded)
                return Ok(new { message = "Registration successful." });

            return BadRequest(result.Errors);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Missing user ID in token");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                username = user.UserName,
                email = user.Email,
                roles = roles
            });
        }
    }
}
