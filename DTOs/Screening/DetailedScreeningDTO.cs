using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Screening
{
    public class DetailedScreeningDTO : BasicScreeningDTO
    {
        public string ScreeningRoomName { get; set; }
        public ScreenFormats ScreeningRoomFormat { get; set; }
        public decimal BasePrice { get; set; }
        public string MovieDescription { get; set; }
        public AgeCategory AgeCategory { get; set; }
    }
}
