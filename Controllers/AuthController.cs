using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // TODO: Walidacja użytkownika
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string email)
        {
            // TODO: Rejestracja użytkownika
            return RedirectToAction("Login");
        }
    }
}
