using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class EditMovieService : IEditMovieService
    {
        private readonly CinemaDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EditMovieService> _logger;

        public EditMovieService(CinemaDbContext db, IWebHostEnvironment env, ILogger<EditMovieService> logger)
        {
            _db = db;
            _env = env;
            _logger = logger;
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy filmów do edycji.");
                var result = await _db.Movies.ToListAsync();
                _logger.LogInformation("Pobrano {Count} filmów.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania listy filmów.");
                return new List<Movie>();
            }
        }

        public async Task<EditMovieDTO?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Pobieranie szczegółów filmu o ID: {Id}", id);
                var movie = await _db.Movies.FindAsync(id);

                if (movie == null)
                {
                    _logger.LogWarning("Nie znaleziono filmu o ID: {Id}", id);
                    return null;
                }

                return new EditMovieDTO
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    MovieLength = movie.MovieLength,
                    AgeCategory = movie.AgeCategory,
                    ExistingImagePath = movie.ImagePath,
                    HasScreenings = movie.Screenings.Any()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania danych filmu o ID: {Id}", id);
                return null;
            }
        }

        public async Task<bool> UpdateMovieAsync(int id, EditMovieDTO dto)
        {
            try
            {
                _logger.LogInformation("Rozpoczęto edycję filmu o ID: {Id}", id);
                var movie = await _db.Movies
                    .Include(m => m.Screenings)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    _logger.LogWarning("Nie znaleziono filmu do edycji o ID: {Id}", id);
                    return false;
                }

                bool hasScreenings = movie.Screenings.Any();

                if (hasScreenings && (movie.Title != dto.Title || movie.MovieLength != dto.MovieLength))
                {
                    _logger.LogWarning("Nie można zmienić tytułu lub długości filmu z zaplanowanymi seansami (ID: {Id}).", id);
                    return false;
                }

                if (!hasScreenings)
                {
                    movie.Title = dto.Title;
                    movie.MovieLength = dto.MovieLength;
                }

                movie.Description = dto.Description;
                movie.AgeCategory = dto.AgeCategory;

                if (dto.PosterFile != null && dto.PosterFile.Length > 0)
                {
                    try
                    {
                        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsDir);

                        var fileName = $"{Guid.NewGuid()}_{dto.PosterFile.FileName}";
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await dto.PosterFile.CopyToAsync(stream);

                        movie.ImagePath = "/uploads/" + fileName;
                        _logger.LogInformation("Zapisano nowy plakat filmu do: {Path}", movie.ImagePath);
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError(fileEx, "Błąd podczas zapisu plakatu dla filmu ID: {Id}", id);
                        return false;
                    }
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation("Pomyślnie zapisano zmiany filmu o ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zapisu zmian filmu o ID: {Id}", id);
                return false;
            }
        }
    }
}
