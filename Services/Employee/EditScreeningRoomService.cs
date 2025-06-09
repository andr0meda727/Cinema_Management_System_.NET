using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
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
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

            if (room == null)
                return false;

            // Sprawdź, czy sala ma zaplanowane seanse w przyszłości
            if (room.Screenings.Any(s => s.DateStartTime > DateTime.Now))
                return false;

            room.Name = dto.Name;
            room.Format = dto.Format;
            room.Rows = dto.Rows;
            room.SeatsPerRow = dto.SeatsPerRow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
