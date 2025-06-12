using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
