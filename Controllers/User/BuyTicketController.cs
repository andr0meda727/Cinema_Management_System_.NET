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
        private readonly ILogger<BuyTicketController> _logger;

        public BuyTicketController(ITicketService ticketService, UserManager<ApplicationUser> userManager, ILogger<BuyTicketController> logger)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("ChooseSeat/{screeningId}")]
        public async Task<IActionResult> ChooseSeat(int screeningId)
        {
            if (screeningId <= 0)
            {
                _logger.LogWarning("Invalid screening ID: {ScreeningId}", screeningId);
                return BadRequest("Invalid screening ID");
            }

            var dto = await _ticketService.GetSeatSelectionAsync(screeningId);

            if (dto == null)
            {
                _logger.LogWarning("Screening not found for ID: {ScreeningId}", screeningId);
                return NotFound("Screening not found");
            }

            _logger.LogInformation("Successfully retrieved seat selection for screening ID: {ScreeningId}", screeningId);
            return View(dto);
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> Buy([FromBody] BuyTicketDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid request data: {ModelState}", ModelState);
                return BadRequest(new { success = false, message = "Invalid request" });
            }


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not authenticated when attempting to purchase tickets");
                return Unauthorized(new { message = "User not authenticated" });
            }

            dto.UserId = user.Id;

            try
            {
                _logger.LogInformation("Attempting to purchase tickets for user {UserId}", user.Id);
                var result = await _ticketService.PurchaseTicketsAsync(dto);

                if (result.Success)
                {
                    _logger.LogInformation("Ticket purchase successful for user {UserId}, ticket IDs: {TicketIds}", user.Id, string.Join(", ", result.TicketIds));
                    return Ok(new
                    {
                        success = true,
                        ticketIds = result.TicketIds,
                    });
                }

                _logger.LogWarning("Ticket purchase failed for user {UserId}, message: {Message}", user.Id, result.Message);
                return BadRequest(new
                {
                    success = false,
                    message = result.Message ?? "Ticket purchase failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ticket purchase for user {UserId}", dto.UserId);
                return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> Summary([FromQuery] string ticketIds)
        {
            if (string.IsNullOrWhiteSpace(ticketIds))
            {
                _logger.LogWarning("No ticket IDs provided");
                return BadRequest("No ticket IDs provided");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not logged in when attempting to view ticket summary");
                return Unauthorized("User not logged in");
            }

            List<int> ids;
            try
            {
                ids = ticketIds.Split(',').Select(int.Parse).ToList();
            }
            catch
            {
                _logger.LogWarning("Invalid ticket ID format: {TicketIds}", ticketIds);
                return BadRequest("Invalid ticket ID format");
            }

            var tickets = await _ticketService.GetTicketSummariesAsync(user.Id, ids);

            if (!tickets.Any() || tickets == null)
            {
                _logger.LogWarning("No tickets found for user {UserId} with ticket IDs: {TicketIds}", user.Id, ticketIds);
                return NotFound("No tickets found");
            }

            _logger.LogInformation("Successfully retrieved ticket summaries for user {UserId}", user.Id);

            ViewBag.TotalPrice = tickets.Sum(t => t.FinalPrice);
            ViewBag.OrderId = tickets.First().Id;

            return View(tickets);
        }
    }
}
