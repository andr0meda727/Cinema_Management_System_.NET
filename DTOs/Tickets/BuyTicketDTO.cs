using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Tickets
{
    public class BuyTicketDTO
    {
        [Required]
        public int ScreeningId { get; set; }

        [Required, MinLength(1)]
        public List<int> SeatIds { get; set; } = new List<int>();

        public string? UserId { get; set; }
    }
}
