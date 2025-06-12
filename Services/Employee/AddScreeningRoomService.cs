using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema_Management_System.Services.Employee
{
    public class AddScreeningRoomService : IAddScreeningRoomService
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<AddScreeningRoomService> _logger;

        public AddScreeningRoomService(CinemaDbContext db, ILogger<AddScreeningRoomService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> AddAsync(CreateScreeningRoomDTO dto)
        {
            try
            {
                _logger.LogInformation("Rozpoczęto dodawanie sali: {Name}", dto.Name);

                bool exists = await _db.ScreeningRooms
                    .AnyAsync(r => r.Name.ToLower() == dto.Name.ToLower());

                if (exists)
                {
                    _logger.LogWarning("Sala o nazwie '{Name}' już istnieje.", dto.Name);
                    return false;
                }

                int totalSeats = dto.Rows * dto.SeatsPerRow;

                var room = new ScreeningRoom
                {
                    Name = dto.Name,
                    Format = dto.Format,
                    Rows = dto.Rows,
                    SeatsPerRow = dto.SeatsPerRow,
                    NumberOfSeats = totalSeats
                };

                _db.ScreeningRooms.Add(room);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Sala '{Name}' została dodana. Generowanie miejsc...", dto.Name);

                var seats = new List<Seat>();
                for (int rowIndex = 0; rowIndex < dto.Rows; rowIndex++)
                {
                    char rowLetter = (char)('A' + rowIndex);
                    SeatTypes type = rowIndex == dto.Rows - 1
                        ? SeatTypes.VIP
                        : rowIndex == dto.Rows - 2
                            ? SeatTypes.DOUBLE
                            : SeatTypes.STANDARD;

                    for (int seatNum = 1; seatNum <= dto.SeatsPerRow; seatNum++)
                    {
                        seats.Add(new Seat
                        {
                            ScreeningRoomId = room.Id,
                            Row = rowLetter.ToString(),
                            SeatInRow = seatNum,
                            SeatType = type
                        });
                    }
                }

                _db.Seats.AddRange(seats);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Wygenerowano {Count} miejsc dla sali '{Name}'.", seats.Count, dto.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania sali: {Name}", dto.Name);
                return false;
            }
        }
    }
}
