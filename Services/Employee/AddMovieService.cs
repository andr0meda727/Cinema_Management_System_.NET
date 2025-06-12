using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;

namespace Cinema_Management_System.Services.Employee
{
    public class AddMovieService : IAddMovieService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<AddMovieService> _logger;

        public AddMovieService(CinemaDbContext db, ILogger<AddMovieService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> AddAsync(AddMovieDTO dto)
        {
            try
            {
                if (_db.Movies.Any(m => m.Title == dto.Title))
                {
                    _logger.LogWarning("Movie with title '{Title}' already exists.", dto.Title);
                    return false;
                }

                _logger.LogInformation("Starting movie creation: {Title}", dto.Title);

                string savedPath = null;

                if (dto.PosterFile != null && dto.PosterFile.Length > 0)
                {
                    try
                    {
                        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(uploadsDir))
                        {
                            _logger.LogInformation("Creating uploads directory at: {Path}", uploadsDir);
                            Directory.CreateDirectory(uploadsDir);
                        }

                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.PosterFile.FileName)}";
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await dto.PosterFile.CopyToAsync(stream);

                        savedPath = fileName;
                        _logger.LogInformation("Poster saved to: {Path}", savedPath);
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError(fileEx, "Error while saving poster for movie: {Title}", dto.Title);
                        return false;
                    }
                }

                var movie = new Movie
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    MovieLength = dto.MovieLength,
                    AgeCategory = dto.AgeCategory,
                    ImagePath = savedPath
                };

                _db.Movies.Add(movie);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Successfully added movie: {Title}", dto.Title);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding movie: {Title}", dto.Title);
                return false;
            }
        }
    }
}
