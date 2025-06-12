using Cinema_Management_System.DTOs.Employee;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IAddScreeningRoomService
    {
        Task<bool> AddAsync(CreateScreeningRoomDTO dto);
    }
}
