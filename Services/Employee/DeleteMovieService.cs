using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteMovieService : IDeleteMovieService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<DeleteMovieService> _logger;

        public DeleteMovieService(CinemaDbContext db, ILogger<DeleteMovieService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<DeleteMovieDTO>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy filmów do usunięcia.");
                var result = await _db.Movies
                    .Select(m => new DeleteMovieDTO
                    {
                        Id = m.Id,
                        Title = m.Title
                    })
                    .ToListAsync();
                _logger.LogInformation("Pobrano {Count} filmów.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania filmów do usunięcia.");
                return new List<DeleteMovieDTO>();
            }
        }

        public async Task<(List<int> DeletedIds, List<int> BlockedIds)> DeleteAsync(List<int> movieIds)
        {
            try
            {
                _logger.LogInformation("Rozpoczęto usuwanie filmów: {Ids}", string.Join(", ", movieIds));

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
                    _logger.LogInformation("Usuwanie filmów: {Ids}", string.Join(", ", deletable.Select(m => m.Id)));
                    _db.Movies.RemoveRange(deletable);
                    await _db.SaveChangesAsync();
                }

                if (blocked.Any())
                {
                    _logger.LogWarning("Zablokowano usunięcie filmów (posiadają przyszłe seanse): {Ids}", string.Join(", ", blocked));
                }

                return (deletable.Select(m => m.Id).ToList(), blocked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania filmów: {Ids}", string.Join(", ", movieIds));
                return (new List<int>(), movieIds); // wszystkie traktujemy jako zablokowane przy błędzie
            }
        }
    }
}
