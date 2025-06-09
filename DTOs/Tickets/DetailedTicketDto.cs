using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Tickets
{
    public class DetailedTicketDTO : BasicTicketDTO
    {
        public required string ScreeningRoomName { get; set; }
        public required string SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public AgeCategory AgeCategory { get; set; }
        public required string MoviePosterUrl { get; set; }
        public DateTime DateEndTime { get; set; }
        public required DateTime PurchaseDate { get; set; }
    }
}
