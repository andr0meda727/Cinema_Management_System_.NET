using Cinema_Management_System.DTOs.Employee;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IDeleteScreeningService
    {
        Task<List<DeleteScreeningDTO>> GetAllAsync();

        Task<(List<int> deleted, List<int> blocked)> DeleteAsync(List<int> ids);
    }
}
