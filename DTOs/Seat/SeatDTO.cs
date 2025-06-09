using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Seat
{
    public class SeatDTO
    {
        public required int Id { get; set; }
        public required string Row { get; set; }
        public int SeatInRow { get; set; }
        public SeatTypes SeatType { get; set; }
        public bool isTaken { get; set; }
        public decimal Price { get; set; }
    }
}
