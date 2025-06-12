using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteScreeningRoomService : IDeleteScreeningRoomService
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
            var now = DateTime.Now;

            var blocked = await _db.ScreeningRooms
                .Where(r => screeningRoomIds.Contains(r.Id))
                .Where(r => _db.Screenings.Any(s => s.ScreeningRoomId == r.Id && s.DateStartTime > now))
                .Select(r => r.Id)
                .ToListAsync();

            var deletable = await _db.ScreeningRooms
                .Where(r => screeningRoomIds.Contains(r.Id) && !blocked.Contains(r.Id))
                .ToListAsync();

            if (deletable.Any())
            {
                _db.ScreeningRooms.RemoveRange(deletable);
                await _db.SaveChangesAsync();
            }

            return (deletable.Select(r => r.Id).ToList(), blocked);
        }
    }
}
