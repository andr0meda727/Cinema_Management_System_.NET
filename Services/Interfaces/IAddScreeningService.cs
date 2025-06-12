using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IAddScreeningService
    {
        Task<List<Movie>> GetMoviesAsync();
        Task<List<ScreeningRoom>> GetRoomsAsync();
        Task<(bool success, string? errorMsg)> AddAsync(AddScreeningDTO dto);
    }
}
