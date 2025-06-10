using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Models.Cinema;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers;

[Mapper]
public partial class SeatSelectionMapper
{
    [MapperIgnoreSource(nameof(Seat.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Seat.ScreeningRoom))]
    [MapperIgnoreTarget(nameof(SeatDTO.Price))]
    [MapperIgnoreTarget(nameof(SeatDTO.isTaken))]
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