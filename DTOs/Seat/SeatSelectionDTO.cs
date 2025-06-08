namespace Cinema_Management_System.DTOs.Seat
{
    public class SeatSelectionDTO
    {
        public int ScreeningId { get; set; }
        public decimal BasePrice { get; set; }
        public List<SeatDTO> Seats { get; set; } = new List<SeatDTO>();
    }
}
