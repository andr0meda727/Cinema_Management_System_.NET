using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.DTOs;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers
{
    [Mapper]
    public partial class TicketMapper
    {
        [MapProperty(nameof(Ticket.Screening.Movie.Title), nameof(BasicTicketDto.MovieTitle))]
        [MapProperty(nameof(Ticket.Screening.DateStartTime), nameof(BasicTicketDto.DateStartTime))]
        public partial BasicTicketDto ToBasicDto(Ticket ticket);
    }
}
