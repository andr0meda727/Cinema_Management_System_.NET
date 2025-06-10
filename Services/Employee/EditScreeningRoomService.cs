using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class EditScreeningRoomService
    {
        private readonly CinemaDbContext _db;

        public EditScreeningRoomService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<EditScreeningRoomDTO>> GetAllAsync()
        {
            return await _db.ScreeningRooms
                .Select(r => new EditScreeningRoomDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Format = r.Format,
                    Rows = r.Rows,
                    SeatsPerRow = r.SeatsPerRow
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(EditScreeningRoomDTO dto)
        {
            var room = await _db.ScreeningRooms
                .Include(r => r.Screenings)
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

            if (room == null)
                return false;

            if (room.Screenings.Any(s => s.DateStartTime > DateTime.Now))
                return false;

            room.Name = dto.Name;
            room.Format = dto.Format;
            room.Rows = dto.Rows;
            room.SeatsPerRow = dto.SeatsPerRow;

            // Usuń stare miejsca
            var oldSeats = _db.Seats.Where(s => s.ScreeningRoomId == room.Id);
            _db.Seats.RemoveRange(oldSeats);

            // Dodaj nowe miejsca
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
                        SeatStatus = false,
                        ScreeningRoom = room
                    });
                }
            }

            await _db.Seats.AddRangeAsync(newSeats);
            room.NumberOfSeats = dto.Rows * dto.SeatsPerRow;

            await _db.SaveChangesAsync();
            return true;
        }


    }
}
