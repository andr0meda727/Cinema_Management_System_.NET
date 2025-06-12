using Cinema_Management_System.DTOs.Employee;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IDeleteScreeningRoomService
    {
        Task<List<DeleteScreeningRoomDTO>> GetAllAsync();

        Task<(List<int> DeletedIds, List<int> BlockedIds)> DeleteAsync(List<int> screeningRoomIds);
    }
}
