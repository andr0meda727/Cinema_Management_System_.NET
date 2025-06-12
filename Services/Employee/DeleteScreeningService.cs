using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteScreeningService : IDeleteScreeningService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<DeleteScreeningService> _logger;

        public DeleteScreeningService(CinemaDbContext db, ILogger<DeleteScreeningService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<DeleteScreeningDTO>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy seansów do usunięcia.");

                var screenings = await _db.Screenings
                    .Include(s => s.Movie)
                    .Include(s => s.ScreeningRoom)
                    .Select(s => new DeleteScreeningDTO
                    {
                        Id = s.Id,
                        MovieTitle = s.Movie.Title,
                        RoomName = s.ScreeningRoom.Name,
                        DateStartTime = s.DateStartTime
                    })
                    .ToListAsync();

                _logger.LogInformation("Pobrano {Count} seansów do usunięcia.", screenings.Count);
                return screenings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania seansów do usunięcia.");
                return new List<DeleteScreeningDTO>();
            }
        }

        public async Task<(List<int> deleted, List<int> blocked)> DeleteAsync(List<int> ids)
        {
            try
            {
                _logger.LogInformation("Rozpoczęto usuwanie seansów: {Ids}", string.Join(", ", ids));

                var screenings = await _db.Screenings
                    .Include(s => s.Tickets)
                    .Where(s => ids.Contains(s.Id))
                    .ToListAsync();

                var deletable = screenings.Where(s => !s.Tickets.Any()).ToList();
                var blocked = screenings.Where(s => s.Tickets.Any()).Select(s => s.Id).ToList();

                if (deletable.Any())
                {
                    _logger.LogInformation("Usuwanie seansów bez biletów: {Ids}", string.Join(", ", deletable.Select(s => s.Id)));
                    _db.Screenings.RemoveRange(deletable);
                    await _db.SaveChangesAsync();
                }

                if (blocked.Any())
                {
                    _logger.LogWarning("Zablokowano usunięcie seansów z aktywnymi biletami: {Ids}", string.Join(", ", blocked));
                }

                return (deletable.Select(s => s.Id).ToList(), blocked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania seansów: {Ids}", string.Join(", ", ids));
                return (new List<int>(), ids); // Wszystkie traktujemy jako zablokowane w razie błędu
            }
        }
    }
}
