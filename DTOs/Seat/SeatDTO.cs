using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Seat
{
    public class SeatDTO
    {
        public string Id { get; set; }
        public string Row { get; set; }
        public int SeatInRow { get; set; }
        public SeatTypes SeatType { get; set; }
        public bool isTaken { get; set; } // 0 - free, 1 - occupied
    }
}
