using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class EditMovieService
    {
        private readonly CinemaDbContext _db;
        private readonly IWebHostEnvironment _env;

        public EditMovieService(CinemaDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _db.Movies.ToListAsync();
        }
        public async Task<EditMovieDTO?> GetByIdAsync(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null) return null;

            return new EditMovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                MovieLength = movie.MovieLength,
                AgeCategory = movie.AgeCategory,
                ExistingImagePath = movie.ImagePath
            };
        }

        public async Task<bool> UpdateMovieAsync(int id, EditMovieDTO dto)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
                return false;

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.MovieLength = dto.MovieLength;
            movie.AgeCategory = dto.AgeCategory;

            if (dto.PosterFile != null && dto.PosterFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}_{dto.PosterFile.FileName}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.PosterFile.CopyToAsync(stream);
                movie.ImagePath = "/uploads/" + fileName;
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
