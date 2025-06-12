using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class AddScreeningService : IAddScreeningService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<AddScreeningService> _logger;

        public AddScreeningService(CinemaDbContext db, ILogger<AddScreeningService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            _logger.LogInformation("Pobieranie listy filmów do formularza tworzenia seansu.");
            return await _db.Movies.ToListAsync();
        }

        public async Task<List<ScreeningRoom>> GetRoomsAsync()
        {
            _logger.LogInformation("Pobieranie listy sal do formularza tworzenia seansu.");
            return await _db.ScreeningRooms.ToListAsync();
        }

        public async Task<(bool success, string? errorMsg)> AddAsync(AddScreeningDTO dto)
        {
            try
            {
                _logger.LogInformation("Próba dodania seansu: FilmId={MovieId}, SalaId={RoomId}, Start={Start}",
                    dto.MovieId, dto.ScreeningRoomId, dto.StartTime);

                var movie = await _db.Movies.FindAsync(dto.MovieId);
                var room = await _db.ScreeningRooms.FindAsync(dto.ScreeningRoomId);

                if (movie == null || room == null)
                {
                    _logger.LogWarning("Nie znaleziono filmu lub sali. MovieId={MovieId}, RoomId={RoomId}",
                        dto.MovieId, dto.ScreeningRoomId);
                    return (false, "Brak filmu lub sali.");
                }

                var endTime = dto.StartTime.AddMinutes(movie.MovieLength);

                if (dto.StartTime < DateTime.Now)
                {
                    _logger.LogWarning("Próba utworzenia seansu w przeszłości. Data: {Start}", dto.StartTime);
                    return (false, "Nie można tworzyć w przeszłości.");
                }

                var conflict = await _db.Screenings
                    .Where(s => s.ScreeningRoomId == dto.ScreeningRoomId)
                    .AnyAsync(s =>
                        dto.StartTime < s.DateEndTime &&
                        endTime > s.DateStartTime);

                if (conflict)
                {
                    _logger.LogWarning("Wykryto konflikt czasowy z innym seansem w tej samej sali.");
                    return (false, "Konflikt z instniejącym seansem o tej godzinie.");
                }

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

                _logger.LogInformation("Seans został pomyślnie dodany: FilmId={MovieId}, SalaId={RoomId}", dto.MovieId, dto.ScreeningRoomId);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania seansu: FilmId={MovieId}, SalaId={RoomId}", dto.MovieId, dto.ScreeningRoomId);
                return (false, "Wystąpił błąd serwera podczas dodawania seansu.");
            }
        }
    }
}
