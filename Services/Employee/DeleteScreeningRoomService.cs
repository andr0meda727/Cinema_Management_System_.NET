using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteScreeningRoomService
    {
        private readonly CinemaDbContext _db;

        public DeleteScreeningRoomService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<DeleteScreeningRoomDTO>> GetAllAsync()
        {
            return await _db.ScreeningRooms
                  .Select(m => new DeleteScreeningRoomDTO
                  {
                      Id = m.Id,
                      Name = m.Name
                  })
                  .ToListAsync();
        }

        public async Task DeleteAsync(List<int> screeningRoomIds)
        {
            var screeningRoomsToRemove = await _db.ScreeningRooms
               .Where(m => screeningRoomIds.Contains(m.Id))
               .ToListAsync();

            _db.ScreeningRooms.RemoveRange(screeningRoomsToRemove);
            await _db.SaveChangesAsync();
        }
    }
}
