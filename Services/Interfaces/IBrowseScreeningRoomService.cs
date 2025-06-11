using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IBrowseScreeningRoomService
    {
        Task<List<ScreeningRoom>> GetAllAsync();
        Task<ScreeningRoomDetailsDTO?> GetByIdAsync(int id);
    }
}
