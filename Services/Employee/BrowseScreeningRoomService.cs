using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class BrowseScreeningRoomService
    {
        private readonly CinemaDbContext _db;

        public BrowseScreeningRoomService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<ScreeningRoom>> GetAllAsync()
        {
            return await _db.ScreeningRooms.ToListAsync();
        }

        public async Task<ScreeningRoomDetailsDTO?> GetByIdAsync(int id)
        {
            var room = await _db.ScreeningRooms
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return null;

            return new ScreeningRoomDetailsDTO
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
        }
    }
}
