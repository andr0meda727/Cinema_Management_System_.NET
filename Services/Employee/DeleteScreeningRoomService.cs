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

        public async Task<(List<int> DeletedIds, List<int> BlockedIds)> DeleteAsync(List<int> screeningRoomIds)
        {
            var roomsToDelete = await _db.ScreeningRooms
                .Include(r => r.Screenings)
                .Where(r => screeningRoomIds.Contains(r.Id))
                .ToListAsync();

            var now = DateTime.Now;

            var blocked = roomsToDelete
                .Where(r => r.Screenings.Any(s => s.DateStartTime > now))
                .Select(r => r.Id)
                .ToList();

            var deletable = roomsToDelete
                .Where(r => !blocked.Contains(r.Id))
                .ToList();

            if (deletable.Any())
            {
                _db.ScreeningRooms.RemoveRange(deletable);
                await _db.SaveChangesAsync();
            }

            return (deletable.Select(r => r.Id).ToList(), blocked);
        }
    }
}
