using Cinema_Management_System.DTOs.Auth;
using Cinema_Management_System.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    public class AuthController : Controller
    {

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
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
            //TODO: fix this and implement DTO
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                Console.WriteLine("Zalogowano poprawnie!");
                return RedirectToAction("Index", "Home");
            }
            else {
                Console.WriteLine("error");
            }

            ModelState.AddModelError(string.Empty, "Nieprawidłowy login lub hasło.");
            return View();
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
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = DateTime.Now,
                RoleId = 1 //  default User
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
       
    }
}
