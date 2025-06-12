using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.DTOs.Screening;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IEditScreeningService
    {
        Task<List<DetailedScreeningDTO>> GetAllAsync();
        Task<EditScreeningDTO?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(EditScreeningDTO dto);
    }
}
