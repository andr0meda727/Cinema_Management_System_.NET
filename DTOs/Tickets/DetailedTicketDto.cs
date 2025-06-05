using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Tickets
{
    public class DetailedTicketDto : BasicTicketDto
    {
        public string ScreeningRoomName { get; set; }
        public string SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public AgeCategory AgeCategory { get; set; }
        public string MoviePosterUrl { get; set; }
        public DateTime DateEndTime { get; set; }
    }
}
