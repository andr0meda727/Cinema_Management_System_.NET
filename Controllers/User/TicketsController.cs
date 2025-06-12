using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
 
namespace Cinema_Management_System.Controllers.User
{
    [Route("Tickets")]
    [Authorize(Roles = "User")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketPdfService _ticketPdfService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(ITicketService ticketService, UserManager<ApplicationUser> userManager, ITicketPdfService ticketPdfService)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _ticketPdfService = ticketPdfService;
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

        [HttpPost("Refund/{ticketId}")]
        [Authorize]
        public async Task<IActionResult> RefundTicket(int ticketId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _ticketService.RefundTicketAsync(user.Id, ticketId);

            if (!result)
            {
                TempData["ErrorMessage"] = "Nie można zwrócić biletu";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Bilet został pomyślnie zwrócony";
            return RedirectToAction("Index");
        }

        [Route("PDF/{ticketId}")]
        public async Task<IActionResult> TicketPDF(int ticketId)
        {  
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var ticket = await _ticketService.GetUserDetailedTicketAsync(user.Id, ticketId);
            if (ticket == null) return NotFound();

            var stream = _ticketPdfService.GeneratePdf(ticket);

            return File(stream, "application/pdf", $"ticket_{ticket.Id}.pdf");
        }
    }
}
