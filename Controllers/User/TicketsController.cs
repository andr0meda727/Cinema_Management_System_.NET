using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers.User
{
    [Route("Tickets")]
    public class TicketsController : Controller
    {
        private readonly TicketService _ticketService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(TicketService ticketService, UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound(); // !!!

            var tickets = await _ticketService.GetUserBasicTicketsAsync(user.Id);

            return View(tickets);
        }

        [Route("Details/{ticketId}")]
        public async Task<IActionResult> Details(int ticketId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound(); // !!!

            var ticket = await _ticketService.GetUserDetailedTicketAsync(user.Id, ticketId);

            return View(ticket);
        }
    }
}
