using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Screening
{
    public class SeatScreeningDTO
    {
        public int Id { get; set; }
        public decimal BasePrice { get; set; }
        public ScreeningRoom? ScreeningRoom { get; set; }
    }

}
