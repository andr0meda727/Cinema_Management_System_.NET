using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class EditScreeningService : IEditScreeningService
{
    private readonly CinemaDbContext _db;
    private readonly ILogger<EditScreeningService> _logger;

    public EditScreeningService(CinemaDbContext db, ILogger<EditScreeningService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<DetailedScreeningDTO>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Pobieranie listy seansów do edycji...");
            var result = await _db.Screenings
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom)
                .Select(s => new DetailedScreeningDTO
                {
                    Id = s.Id,
                    Title = s.Movie.Title,
                    DateStartTime = s.DateStartTime,
                    DateEndTime = s.DateEndTime,
                    ShortDescription = s.Movie.Description.Length > 100
                        ? s.Movie.Description.Substring(0, 100) + "..."
                        : s.Movie.Description,
                    MovieDescription = s.Movie.Description,
                    AgeCategory = s.Movie.AgeCategory,
                    MoviePosterUrl = s.Movie.ImagePath,
                    ScreeningRoomName = s.ScreeningRoom.Name,
                    ScreeningRoomFormat = s.ScreeningRoom.Format,
                    BasePrice = s.BasePrice
                })
                .ToListAsync();

            _logger.LogInformation("Znaleziono {Count} seansów.", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania listy seansów.");
            return new List<DetailedScreeningDTO>();
        }
    }

    public async Task<EditScreeningDTO?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Pobieranie seansu o ID {Id} do edycji.", id);

            var screening = await _db.Screenings.FindAsync(id);
            if (screening == null)
            {
                _logger.LogWarning("Nie znaleziono seansu o ID {Id}.", id);
                return null;
            }

            var hasTickets = await _db.Tickets.AnyAsync(t => t.ScreeningId == id);
            if (hasTickets)
            {
                _logger.LogWarning("Seans o ID {Id} ma przypisane bilety – edycja zablokowana.", id);
                return null;
            }

            return new EditScreeningDTO
            {
                Id = screening.Id,
                MovieId = screening.MovieId,
                ScreeningRoomId = screening.ScreeningRoomId,
                DateStartTime = screening.DateStartTime,
                BasePrice = screening.BasePrice
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania seansu o ID {Id}.", id);
            return null;
        }
    }

    public async Task<bool> UpdateAsync(EditScreeningDTO dto)
    {
        try
        {
            _logger.LogInformation("Aktualizacja seansu ID {Id} – start: {Start}, sala: {RoomId}, film: {MovieId}",
                dto.Id, dto.DateStartTime, dto.ScreeningRoomId, dto.MovieId);

            var screening = await _db.Screenings
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (screening == null || screening.Tickets.Any())
            {
                _logger.LogWarning("Nie można zaktualizować – seans nie istnieje lub posiada bilety.");
                return false;
            }

            var movie = await _db.Movies.FindAsync(dto.MovieId);
            if (movie == null)
            {
                _logger.LogWarning("Nie znaleziono filmu o ID {MovieId}", dto.MovieId);
                return false;
            }

            var proposedStart = dto.DateStartTime;
            var proposedEnd = proposedStart.AddMinutes(movie.MovieLength);

            if (proposedStart < DateTime.Now)
            {
                _logger.LogWarning("Seans nie może być zaplanowany w przeszłości.");
                return false;
            }

            var hasConflict = await _db.Screenings
                .Where(s => s.Id != dto.Id && s.ScreeningRoomId == dto.ScreeningRoomId)
                .AnyAsync(s =>
                    proposedStart < s.DateEndTime &&
                    proposedEnd > s.DateStartTime);

            if (hasConflict)
            {
                _logger.LogWarning("Konflikt czasowy z innym seansem w tej samej sali.");
                return false;
            }

            screening.DateStartTime = proposedStart;
            screening.DateEndTime = proposedEnd;
            screening.BasePrice = dto.BasePrice;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Seans ID {Id} został pomyślnie zaktualizowany.", dto.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji seansu ID {Id}.", dto.Id);
            return false;
        }
    }
}
