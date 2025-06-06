using Cinema_Management_System.Models.Cinema;
using Riok.Mapperly.Abstractions;
using Cinema_Management_System.DTOs.Tickets;

namespace Cinema_Management_System.Mappers
{
    [Mapper]
    public partial class TicketMapper
    {
        [MapProperty("Screening.Movie.Title", nameof(BasicTicketDto.MovieTitle))]
        [MapProperty("Screening.DateStartTime", nameof(BasicTicketDto.DateStartTime))]
        public partial BasicTicketDto ToBasicDto(Ticket ticket);

        [MapProperty("Screening.Movie.Title", nameof(BasicTicketDto.MovieTitle))]
        [MapProperty("Screening.DateStartTime", nameof(BasicTicketDto.DateStartTime))]
        [MapProperty("Screening.ScreeningRoom.Name", nameof(DetailedTicketDto.ScreeningRoomName))]
        [MapProperty("Seat.Row", nameof(DetailedTicketDto.SeatRow))]
        [MapProperty("Seat.SeatNumber", nameof(DetailedTicketDto.SeatNumber))]
        [MapProperty("Screening.Movie.AgeCategory", nameof(DetailedTicketDto.AgeCategory))]
        [MapProperty("Screening.Movie.ImagePath", nameof(DetailedTicketDto.MoviePosterUrl))]
        [MapProperty("Screening.DateEndTime", nameof(DetailedTicketDto.DateEndTime))]
        public partial DetailedTicketDto ToDetailedDto(Ticket ticket);

        //public partial List<BasicTicketDto> ToDtoList(IEnumerable<Ticket> tickets);
    }
}
