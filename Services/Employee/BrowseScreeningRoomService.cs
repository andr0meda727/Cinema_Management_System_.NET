using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class BrowseScreeningRoomService : IBrowseScreeningRoomService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<BrowseScreeningRoomService> _logger;

        public BrowseScreeningRoomService(CinemaDbContext db, ILogger<BrowseScreeningRoomService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<ScreeningRoom>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Rozpoczęto pobieranie listy sal kinowych.");
                var rooms = await _db.ScreeningRooms.ToListAsync();
                _logger.LogInformation("Pobrano {Count} sal kinowych z bazy danych.", rooms.Count);
                return rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania listy sal kinowych.");
                return new List<ScreeningRoom>();
            }
        }

        public async Task<ScreeningRoomDetailsDTO?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Pobieranie szczegółów sali o ID: {Id}", id);

                var room = await _db.ScreeningRooms
                    .Include(r => r.Seats)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (room == null)
                {
                    _logger.LogWarning("Nie znaleziono sali o ID: {Id}", id);
                    return null;
                }

                var dto = new ScreeningRoomDetailsDTO
                {
                    Id = room.Id,
                    Name = room.Name,
                    Format = room.Format,
                    Rows = room.Rows,
                    SeatsPerRow = room.SeatsPerRow,
                    Seats = room.Seats.Select(s => new SeatDTO
                    {
                        Row = s.Row,
                        SeatInRow = s.SeatInRow,
                        SeatType = s.SeatType
                    }).ToList()
                };

                _logger.LogInformation("Pomyślnie pobrano szczegóły sali o ID: {Id}", id);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania szczegółów sali o ID: {Id}", id);
                return null;
            }
        }
    }
}
