using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteScreeningService : IDeleteScreeningService
    {
        private readonly CinemaDbContext _db;

        public DeleteScreeningService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<DeleteScreeningDTO>> GetAllAsync()
        {
            return await _db.Screenings
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom)
                .Select(s => new DeleteScreeningDTO
                {
                    Id = s.Id,
                    MovieTitle = s.Movie.Title,
                    RoomName = s.ScreeningRoom.Name,
                    DateStartTime = s.DateStartTime
                })
                .ToListAsync();
        }

        public async Task<(List<int> deleted, List<int> blocked)> DeleteAsync(List<int> ids)
        {
            var screenings = await _db.Screenings
                .Include(s => s.Tickets)
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();

            var deletable = screenings.Where(s => !s.Tickets.Any()).ToList();
            var blocked = screenings.Where(s => s.Tickets.Any()).Select(s => s.Id).ToList();

            _db.Screenings.RemoveRange(deletable);
            await _db.SaveChangesAsync();

            return (deletable.Select(s => s.Id).ToList(), blocked);
        }
    }
}
