using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Employee
{
    public class AddMovieService
    {
        private readonly CinemaDbContext _db;

        public AddMovieService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(AddMovieDTO dto)
        {
            bool exists = _db.Movies
            .Any(m => m.Title.ToLower() == dto.Title.ToLower());

            if (exists)
                return false;

            var movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                MovieLength = dto.MovieLength,
                AgeCategory = dto.AgeCategory,
                ImagePath = dto.ImagePath
            };

            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
