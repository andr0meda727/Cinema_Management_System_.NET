using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class AddScreeningService
    {
        private readonly CinemaDbContext _db;

        public AddScreeningService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            return await _db.Movies.ToListAsync();
        }

        public async Task<List<ScreeningRoom>> GetRoomsAsync()
        {
            return await _db.ScreeningRooms.ToListAsync();
        }
        public async Task<(bool success, string? errorMsg)> AddAsync(AddScreeningDTO dto)
        {
            var movie = await _db.Movies.FindAsync(dto.MovieId);
            var room = await _db.ScreeningRooms.FindAsync(dto.ScreeningRoomId);

            if (movie == null || room == null)
                return (false, "Movie or room not found.");

            var endTime = dto.StartTime.AddMinutes(movie.MovieLength);

            if (dto.StartTime < DateTime.Now)
                return (false, "Cannot add screening in the past.");

            var conflict = await _db.Screenings
                .Where(s => s.ScreeningRoomId == dto.ScreeningRoomId)
                .AnyAsync(s =>
                    dto.StartTime < s.DateEndTime &&
                    endTime > s.DateStartTime
                );

            if (conflict)
                return (false, "Another screening overlaps with the selected time.");

            var screening = new Screening
            {
                MovieId = dto.MovieId,
                ScreeningRoomId = dto.ScreeningRoomId,
                DateStartTime = dto.StartTime,
                DateEndTime = endTime,
                BasePrice = dto.BasePrice,
                Movie = movie,
                ScreeningRoom = room
            };

            _db.Screenings.Add(screening);
            await _db.SaveChangesAsync();
            return (true, null);
        }

    }
}
