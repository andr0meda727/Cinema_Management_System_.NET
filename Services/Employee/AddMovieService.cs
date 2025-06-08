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
            //check if film aready exists
            if (_db.Movies.Any(m => m.Title == dto.Title))
                return false;

            string? savedPath = null;

            if (dto.PosterFile != null && dto.PosterFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}_{dto.PosterFile.FileName}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PosterFile.CopyToAsync(stream);
                }

                savedPath = "/uploads/" + fileName;
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
            return true;
        }
    }
}
