using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Models.Cinema;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers;

[Mapper]
public partial class SeatSelectionMapper
{
    [MapProperty(nameof(Seat.SeatStatus), nameof(SeatDTO.isTaken))]
    [MapperIgnoreSource(nameof(Seat.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Seat.ScreeningRoom))]
    [MapperIgnoreSource(nameof(Seat.Ticket))]
    [MapperIgnoreTarget(nameof(SeatDTO.Price))]
    public partial SeatDTO SeatToSeatDTO(Seat seat);

    public partial List<SeatDTO> MapSeats(List<Seat> seats);

    [MapperIgnoreSource(nameof(Screening.MovieId))]
    [MapperIgnoreSource(nameof(Screening.Movie))]
    [MapperIgnoreSource(nameof(Screening.DateStartTime))]
    [MapperIgnoreSource(nameof(Screening.DateEndTime))]
    [MapperIgnoreSource(nameof(Screening.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Screening.Tickets))]
    public partial SeatScreeningDTO ScreeningToSeatScreeningDTO(Screening screening);

    public SeatSelectionDTO ScreeningToSeatSelectionDTO(Screening screening, List<Seat> seats)
    {
        var dto = new SeatSelectionDTO
        {
            Screening = ScreeningToSeatScreeningDTO(screening),
            Seats = MapSeats(seats),
            BasePrice = screening.BasePrice
        };
        return dto;
    }
}