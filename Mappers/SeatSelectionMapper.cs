using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.Models.Cinema;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers;

[Mapper]
public partial class SeatSelectionMapper
{
    [MapProperty("SeatStatus", nameof(SeatDTO.isTaken))]
    [MapperIgnoreSource(nameof(Seat.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Seat.ScreeningRoom))]
    [MapperIgnoreSource(nameof(Seat.Ticket))]
    public partial SeatDTO SeatToSeatDTO(Seat seat);

    public partial List<SeatDTO> MapSeats(List<Seat> seats);

    [MapProperty("Id", nameof(SeatSelectionDTO.ScreeningId))]
    [MapperIgnoreSource(nameof(Screening.MovieId))]
    [MapperIgnoreSource(nameof(Screening.Movie))]
    [MapperIgnoreSource(nameof(Screening.DateStartTime))]
    [MapperIgnoreSource(nameof(Screening.DateEndTime))]
    [MapperIgnoreSource(nameof(Screening.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Screening.ScreeningRoom))]
    [MapperIgnoreSource(nameof(Screening.Tickets))]
    public partial SeatSelectionDTO ScreeningToSeatSelectionDTO(Screening screening, List<Seat> seats);
}
