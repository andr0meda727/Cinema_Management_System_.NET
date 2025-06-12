using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Cinema_Management_System.Controllers.User
{
    [Route("BuyTicket")]
    public class BuyTicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuyTicketController(ITicketService ticketService, UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("ChooseSeat/{screeningId}")]
        public async Task<IActionResult> ChooseSeat(int screeningId)
        {
            var dto = await _ticketService.GetSeatSelectionAsync(screeningId);
            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        [HttpPost("Buy")] //vlidateantiforgerytoken
        public async Task<IActionResult> Buy([FromBody] BuyTicketDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "Użytkownik niezalogowany." });
            }

            dto.UserId = user.Id;

            var result = await _ticketService.PurchaseTicketsAsync(dto);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    ticketIds = result.TicketIds,
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message ?? "Wystąpił błąd podczas przetwarzania zamówienia."
            });
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> Summary([FromQuery] string ticketIds)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not logged in");
            }

            if (string.IsNullOrEmpty(ticketIds))
            {
                return BadRequest("No ticket IDs provided");
            }

            var ids = ticketIds.Split(',').Select(int.Parse).ToList();
            var tickets = await _ticketService.GetTicketSummariesAsync(user.Id, ids);

            if (!tickets.Any())
            {
                return NotFound("No tickets found");
            }

            ViewBag.TotalPrice = tickets.Sum(t => t.FinalPrice);
            ViewBag.OrderId = tickets.First().Id;

            return View(tickets);
        }
    }
}
