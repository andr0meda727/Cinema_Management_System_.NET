using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class BrowseScreeningService : IBrowseScreeningService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<BrowseScreeningService> _logger;

        public BrowseScreeningService(CinemaDbContext db, ILogger<BrowseScreeningService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<BrowseScreeningDTO>> GetScreeningsByDateAsync(DateTime date)
        {
            try
            {
                DateTime startDate = date.Date;
                DateTime endDate = startDate.AddDays(1);

                _logger.LogInformation("Pobieranie seansów z daty: {Date} (od {Start} do {End})", date.ToShortDateString(), startDate, endDate);

                var screenings = await _db.Screenings
                    .Include(s => s.Movie)
                    .Include(s => s.ScreeningRoom)
                    .Where(s => s.DateStartTime >= startDate && s.DateStartTime < endDate)
                    .Select(s => new BrowseScreeningDTO
                    {
                        Id = s.Id,
                        MovieTitle = s.Movie.Title,
                        RoomName = s.ScreeningRoom.Name,
                        DateStartTime = s.DateStartTime,
                        DateEndTime = s.DateEndTime,
                        BasePrice = s.BasePrice
                    })
                    .ToListAsync();

                _logger.LogInformation("Pobrano {Count} seansów na dzień {Date}.", screenings.Count, date.ToShortDateString());
                return screenings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania seansów z dnia: {Date}", date.ToShortDateString());
                return new List<BrowseScreeningDTO>();
            }
        }
    }
}
