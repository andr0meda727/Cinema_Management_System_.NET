using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class DeleteScreeningRoomService : IDeleteScreeningRoomService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<DeleteScreeningRoomService> _logger;

        public DeleteScreeningRoomService(CinemaDbContext db, ILogger<DeleteScreeningRoomService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<DeleteScreeningRoomDTO>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie listy sal kinowych do usunięcia.");
                var result = await _db.ScreeningRooms
                    .Select(r => new DeleteScreeningRoomDTO
                    {
                        Id = r.Id,
                        Name = r.Name
                    })
                    .ToListAsync();
                _logger.LogInformation("Pobrano {Count} sal kinowych.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania sal kinowych do usunięcia.");
                return new List<DeleteScreeningRoomDTO>();
            }
        }

        public async Task<(List<int> DeletedIds, List<int> BlockedIds)> DeleteAsync(List<int> screeningRoomIds)
        {
            try
            {
                _logger.LogInformation("Rozpoczęto usuwanie sal: {Ids}", string.Join(", ", screeningRoomIds));

                var now = DateTime.Now;

                var blocked = await _db.ScreeningRooms
                    .Where(r => screeningRoomIds.Contains(r.Id))
                    .Where(r => _db.Screenings.Any(s => s.ScreeningRoomId == r.Id && s.DateStartTime > now))
                    .Select(r => r.Id)
                    .ToListAsync();

                var deletable = await _db.ScreeningRooms
                    .Where(r => screeningRoomIds.Contains(r.Id) && !blocked.Contains(r.Id))
                    .ToListAsync();

                if (deletable.Any())
                {
                    _logger.LogInformation("Usuwanie sal: {Ids}", string.Join(", ", deletable.Select(r => r.Id)));
                    _db.ScreeningRooms.RemoveRange(deletable);
                    await _db.SaveChangesAsync();
                }

                if (blocked.Any())
                {
                    _logger.LogWarning("Zablokowano usunięcie sal (mają przyszłe seanse): {Ids}", string.Join(", ", blocked));
                }

                return (deletable.Select(r => r.Id).ToList(), blocked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania sal: {Ids}", string.Join(", ", screeningRoomIds));
                return (new List<int>(), screeningRoomIds); // traktujemy jako zablokowane w razie błędu
            }
        }
    }
}
