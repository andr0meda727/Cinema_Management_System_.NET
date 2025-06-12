using Cinema_Management_System.DTOs.Employee;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IBrowseScreeningService
    {
        Task<List<BrowseScreeningDTO>> GetScreeningsByDateAsync(DateTime date);

    }
}
