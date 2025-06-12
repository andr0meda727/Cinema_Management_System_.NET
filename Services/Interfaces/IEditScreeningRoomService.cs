using Cinema_Management_System.DTOs.Employee;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IEditScreeningRoomService
    {
        Task<List<EditScreeningRoomDTO>> GetAllAsync();
        Task<bool> UpdateAsync(EditScreeningRoomDTO dto);
    }
}
