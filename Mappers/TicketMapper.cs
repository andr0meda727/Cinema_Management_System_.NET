using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Models.Cinema;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers
{
    [Mapper]
    public partial class TicketMapper
    {
        [MapProperty("Screening.Movie.Title", nameof(BasicTicketDTO.MovieTitle))]
        [MapProperty("Screening.DateStartTime", nameof(BasicTicketDTO.DateStartTime))]
        public partial BasicTicketDTO TicketToTicketBasicDTO(Ticket ticket);

        [MapProperty("Screening.Movie.Title", nameof(BasicTicketDTO.MovieTitle))]
        [MapProperty("Screening.DateStartTime", nameof(BasicTicketDTO.DateStartTime))]
        [MapProperty("Screening.ScreeningRoom.Name", nameof(DetailedTicketDTO.ScreeningRoomName))]
        [MapProperty("Seat.Row", nameof(DetailedTicketDTO.SeatRow))]
        [MapProperty("Seat.SeatNumber", nameof(DetailedTicketDTO.SeatNumber))]
        [MapProperty("Screening.Movie.AgeCategory", nameof(DetailedTicketDTO.AgeCategory))]
        [MapProperty("Screening.Movie.ImagePath", nameof(DetailedTicketDTO.MoviePosterUrl))]
        [MapProperty("Screening.DateEndTime", nameof(DetailedTicketDTO.DateEndTime))]
        public partial DetailedTicketDTO TicketToTicketDetailedDTO(Ticket ticket);
    }
}
