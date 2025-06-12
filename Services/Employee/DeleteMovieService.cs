using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteMovieService : IDeleteMovieService
    {
        private readonly CinemaDbContext _db;

        public DeleteMovieService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<DeleteMovieDTO>> GetAllAsync()
        {
            return await _db.Movies
                  .Select(m => new DeleteMovieDTO
                  {
                      Id = m.Id,
                      Title = m.Title
                  })
                  .ToListAsync();
        }

        public async Task<(List<int> DeletedIds, List<int> BlockedIds)> DeleteAsync(List<int> movieIds)
        {
            var movies = await _db.Movies
                .Include(m => m.Screenings)
                .Where(m => movieIds.Contains(m.Id))
                .ToListAsync();

            var now = DateTime.Now;

            var blocked = movies
                .Where(m => m.Screenings.Any(s => s.DateStartTime > now))
                .Select(m => m.Id)
                .ToList();

            var deletable = movies
                .Where(m => !blocked.Contains(m.Id))
                .ToList();

            if (deletable.Any())
            {
                _db.Movies.RemoveRange(deletable);
                await _db.SaveChangesAsync();
            }

            return (deletable.Select(m => m.Id).ToList(), blocked);
        }
    }
}
