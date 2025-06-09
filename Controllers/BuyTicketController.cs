using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    [Route("BuyTicket")]
    public class BuyTicketController : Controller
    {
        private readonly TicketService _ticketService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuyTicketController(TicketService seatSelectionService, UserManager<ApplicationUser> userManager)
        {
            _ticketService = seatSelectionService;
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

        [HttpPost("Buy")]
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
                    orderId = result.OrderId
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message ?? "Wystąpił błąd podczas przetwarzania zamówienia."
            });
        }
    }
}
