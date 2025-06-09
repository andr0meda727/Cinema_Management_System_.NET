using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Models.Cinema;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers;

[Mapper]
public partial class TicketMapper
{
    [MapProperty("Screening.Movie.Title", nameof(BasicTicketDTO.MovieTitle))]
    [MapProperty("Screening.DateStartTime", nameof(BasicTicketDTO.DateStartTime))]
    [MapperIgnoreSource(nameof(Ticket.ScreeningId))]
    [MapperIgnoreSource(nameof(Ticket.SeatId))]
    [MapperIgnoreSource(nameof(Ticket.Seat))]
    [MapperIgnoreSource(nameof(Ticket.UserId))]
    [MapperIgnoreSource(nameof(Ticket.User))]
    [MapperIgnoreSource(nameof(Ticket.PurchaseDate))]
    public partial BasicTicketDTO TicketToTicketBasicDTO(Ticket ticket);

    [MapProperty("Screening.Movie.Title", nameof(DetailedTicketDTO.MovieTitle))]
    [MapProperty("Screening.DateStartTime", nameof(DetailedTicketDTO.DateStartTime))]
    [MapProperty("Screening.DateEndTime", nameof(DetailedTicketDTO.DateEndTime))]
    [MapProperty("Screening.Movie.AgeCategory", nameof(DetailedTicketDTO.AgeCategory))]
    [MapProperty("Screening.Movie.ImagePath", nameof(DetailedTicketDTO.MoviePosterUrl))]
    [MapProperty("Screening.ScreeningRoom.Name", nameof(DetailedTicketDTO.ScreeningRoomName))]
    [MapProperty("Seat.Row", nameof(DetailedTicketDTO.SeatRow))]
    [MapProperty("Seat.SeatInRow", nameof(DetailedTicketDTO.SeatNumber))]
    [MapperIgnoreSource(nameof(Ticket.User))]
    [MapperIgnoreSource(nameof(Ticket.UserId))]
    [MapperIgnoreSource(nameof(Ticket.ScreeningId))]
    [MapperIgnoreSource(nameof(Ticket.SeatId))]
    public partial DetailedTicketDTO TicketToTicketDetailedDTO(Ticket ticket);
}
