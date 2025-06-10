using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Employee
{
    public class ScreeningRoomDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ScreenFormats Format { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public List<SeatDTO> Seats { get; set; } = new();
    }

    public class SeatDTO
    {
        public string Row { get; set; }
        public int SeatInRow { get; set; }
        public SeatTypes SeatType { get; set; }
    }
}
