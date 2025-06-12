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
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, UserManager<ApplicationUser> userManager, ITicketPdfService ticketPdfService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _ticketPdfService = ticketPdfService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("User not found when accessing tickets page.");
                return NotFound();
            }

            var tickets = await _ticketService.GetUserBasicTicketsAsync(user.Id);

            _logger.LogInformation("Retrieved {TicketCount} tickets for user {UserId}", tickets.Count, user.Id);

            return View(tickets);
        }

        [Route("Details/{ticketId}")]
        public async Task<IActionResult> Details(int ticketId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("User not found when accessing ticket details for ticketId: {TicketId}", ticketId);
                return NotFound();
            }

            var ticket = await _ticketService.GetUserDetailedTicketAsync(user.Id, ticketId);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found for user {UserId} with ticketId: {TicketId}", user.Id, ticketId);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved details for ticketId: {TicketId} for user {UserId}", ticketId, user.Id);

            return View(ticket);
        }

        [HttpPost("Refund/{ticketId}")]
        [Authorize]
        public async Task<IActionResult> RefundTicket(int ticketId)
        {
           var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not authenticated when attempting to refund ticketId: {TicketId}", ticketId);
                return Unauthorized();
            }

            var result = await _ticketService.RefundTicketAsync(user.Id, ticketId);

            if (!result)
            {
                _logger.LogWarning("Refund failed for user {UserId} with ticketId: {TicketId}", user.Id, ticketId);
                TempData["ErrorMessage"] = "Nie można zwrócić biletu";
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Refund successful for user {UserId} with ticketId: {TicketId}", user.Id, ticketId);
            TempData["SuccessMessage"] = "Bilet został pomyślnie zwrócony";
            return RedirectToAction("Index");
        }

        [Route("PDF/{ticketId}")]
        public async Task<IActionResult> TicketPDF(int ticketId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not authenticated when attempting to download PDF for ticketId: {TicketId}", ticketId);
                return Unauthorized();
            }

            var ticket = await _ticketService.GetUserDetailedTicketAsync(user.Id, ticketId);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found for user {UserId} with ticketId: {TicketId}", user.Id, ticketId);
                return NotFound();
            }

            _logger.LogInformation("Successfully generated PDF for ticketId: {TicketId} for user {UserId}", ticketId, user.Id);

            var stream = _ticketPdfService.GeneratePdf(ticket);

            return File(stream, "application/pdf", $"ticket_{ticket.Id}.pdf");
        }
    }
}
