using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteMovieService
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

        public async Task DeleteAsync(List<int> movieIds)
        {
            var moviesToRemove = await _db.Movies
               .Where(m => movieIds.Contains(m.Id))
               .ToListAsync();

            _db.Movies.RemoveRange(moviesToRemove);
            await _db.SaveChangesAsync();
        }
    }
}
