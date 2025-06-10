using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.DTOs.Tickets;


namespace Cinema_Management_System.Services.Interfaces
{
    public interface ITicketService
    {
        Task<PurchaseResult> PurchaseTicketsAsync(BuyTicketDTO dto);
        Task<SeatSelectionDTO?> GetSeatSelectionAsync(int screeningId);
        Task<List<BasicTicketDTO>> GetUserBasicTicketsAsync(string userId);
        Task<DetailedTicketDTO> GetUserDetailedTicketAsync(string userId, int ticketId);
        Task<List<DetailedTicketDTO>> GetTicketSummariesAsync(List<int> ticketIds);
        Task<bool> RefundTicketAsync(string userId, int ticketId);
    }
}
