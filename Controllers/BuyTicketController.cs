using Cinema_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Management_System.Controllers
{
    public class BuyTicketController : Controller
    {
        private readonly TicketService _ticketService;

        public BuyTicketController(TicketService seatSelectionService)
        {
            _ticketService = seatSelectionService;
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

        //[HttpPost]
        //public IActionResult BuyTicket([FromForm] List<int> seatIds, int screeningId)
        //{
        //return Ok(new { Message = "To be implemented", seatIds, screeningId });
        //}
    }
}
