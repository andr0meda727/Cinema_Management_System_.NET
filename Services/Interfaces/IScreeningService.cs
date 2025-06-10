using Cinema_Management_System.DTOs.Screening;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IScreeningService
    {
        Task<List<BasicScreeningDTO>> GetScreeningsAsyncDate(DateTime date);
        Task<DetailedScreeningDTO?> GetDetailedScreeningByIdAsync(int screeningId);
    }
}
