using Cinema_Management_System.DTOs.Screening;

namespace Cinema_Management_System.DTOs.Seat
{
    public class SeatSelectionDTO
    {
        public SeatScreeningDTO Screening { get; set; } = new SeatScreeningDTO();
        public List<SeatDTO> Seats { get; set; } = new List<SeatDTO>();
        public decimal BasePrice { get; set; }

    }
}
