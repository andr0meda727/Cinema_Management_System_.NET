namespace Cinema_Management_System.DTOs.Tickets
{
    public class BuyTicketDTO
    {
        public int ScreeningId { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();
    }
}
