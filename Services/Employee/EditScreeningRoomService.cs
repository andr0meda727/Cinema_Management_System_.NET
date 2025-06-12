using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class EditScreeningRoomService : IEditScreeningRoomService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<EditScreeningRoomService> _logger;

        public EditScreeningRoomService(CinemaDbContext db, ILogger<EditScreeningRoomService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<EditScreeningRoomDTO>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie wszystkich sal do edycji.");
                var result = await _db.ScreeningRooms
                    .Select(r => new EditScreeningRoomDTO
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Format = r.Format,
                        Rows = r.Rows,
                        SeatsPerRow = r.SeatsPerRow
                    })
                    .ToListAsync();

                _logger.LogInformation("Znaleziono {Count} sal(e).", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania sal.");
                return new List<EditScreeningRoomDTO>();
            }
        }

        public async Task<bool> UpdateAsync(EditScreeningRoomDTO dto)
        {
            try
            {
                _logger.LogInformation("Rozpoczynanie aktualizacji sali o ID: {Id}", dto.Id);

                var room = await _db.ScreeningRooms
                    .Include(r => r.Screenings)
                    .Include(r => r.Seats)
                    .FirstOrDefaultAsync(r => r.Id == dto.Id);

                if (room == null)
                {
                    _logger.LogWarning("Nie znaleziono sali o ID: {Id}", dto.Id);
                    return false;
                }

                if (room.Screenings.Any(s => s.DateStartTime > DateTime.Now))
                {
                    _logger.LogWarning("Sala ID: {Id} ma zaplanowane przyszłe seanse – edycja zablokowana.", dto.Id);
                    return false;
                }

                room.Name = dto.Name;
                room.Format = dto.Format;
                room.Rows = dto.Rows;
                room.SeatsPerRow = dto.SeatsPerRow;

                _logger.LogInformation("Usuwanie starych miejsc dla sali ID: {Id}", dto.Id);
                var oldSeats = _db.Seats.Where(s => s.ScreeningRoomId == room.Id);
                _db.Seats.RemoveRange(oldSeats);

                var newSeats = new List<Seat>();
                for (int i = 0; i < dto.Rows; i++)
                {
                    var rowLetter = ((char)('A' + i)).ToString();
                    for (int j = 1; j <= dto.SeatsPerRow; j++)
                    {
                        newSeats.Add(new Seat
                        {
                            Row = rowLetter,
                            SeatInRow = j,
                            ScreeningRoomId = room.Id,
                            SeatType = SeatTypes.STANDARD,
                            ScreeningRoom = room
                        });
                    }
                }

                _logger.LogInformation("Dodawanie {Count} nowych miejsc do sali ID: {Id}", newSeats.Count, dto.Id);
                await _db.Seats.AddRangeAsync(newSeats);
                room.NumberOfSeats = dto.Rows * dto.SeatsPerRow;

                await _db.SaveChangesAsync();
                _logger.LogInformation("Sala ID: {Id} została pomyślnie zaktualizowana.", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas edycji sali o ID: {Id}", dto.Id);
                return false;
            }
        }
    }
}
