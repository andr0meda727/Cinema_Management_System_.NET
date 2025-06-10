using Cinema_Management_System.Data;
using Cinema_Management_System.Models.Cinema;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class BrowseMoviesService
    {
        private readonly CinemaDbContext _db;

        public BrowseMoviesService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _db.Movies.ToListAsync();
        }
    }
}
