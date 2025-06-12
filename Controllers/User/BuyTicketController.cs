using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Cinema_Management_System.Controllers.User
{
    [Route("BuyTicket")]
    [Authorize(Roles = "User")]
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
            if (screeningId <= 0)
                return BadRequest("Invalid screening ID");

            var dto = await _ticketService.GetSeatSelectionAsync(screeningId);

            if (dto == null)
                return NotFound("Screening not found");

            return View(dto);
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> Buy([FromBody] BuyTicketDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request" });


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            dto.UserId = user.Id;

            try
            {
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
                    message = result.Message ?? "Ticket purchase failed"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                //_logger.LogError(ex, "Error during ticket purchase.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> Summary([FromQuery] string ticketIds)
        {
            if (string.IsNullOrWhiteSpace(ticketIds))
                return BadRequest("No ticket IDs provided");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not logged in");
            }

            List<int> ids;
            try {
                ids = ticketIds.Split(',').Select(int.Parse).ToList();
            } catch
            {
                return BadRequest("Invalid ticket ID format");
            }

            var tickets = await _ticketService.GetTicketSummariesAsync(user.Id, ids);

            if (!tickets.Any() || tickets == null)
            {
                return NotFound("No tickets found");
            }

            ViewBag.TotalPrice = tickets.Sum(t => t.FinalPrice);
            ViewBag.OrderId = tickets.First().Id;

            return View(tickets);
        }
    }
}
