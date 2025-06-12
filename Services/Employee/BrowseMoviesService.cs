using Cinema_Management_System.Data;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class BrowseMoviesService : IBrowseMoviesService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<BrowseMoviesService> _logger;

        public BrowseMoviesService(CinemaDbContext db, ILogger<BrowseMoviesService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            try
            {
                _logger.LogInformation("Rozpoczęto pobieranie listy filmów.");
                var movies = await _db.Movies.ToListAsync();
                _logger.LogInformation("Pobrano {Count} filmów z bazy danych.", movies.Count);
                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania listy filmów.");
                return new List<Movie>();
            }
        }
    }
}
