using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{
    public class AddScreeningRoomService : IAddScreeningRoomService
    {
        private readonly CinemaDbContext _db;

        public AddScreeningRoomService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(CreateScreeningRoomDTO dto)
        {
            // Check if a room with the same name already exists (case-insensitive)
            bool exists = await _db.ScreeningRooms
                .AnyAsync(r => r.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return false;

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

            var seats = new List<Seat>();
            for (int rowIndex = 0; rowIndex < dto.Rows; rowIndex++)
            {
                char rowLetter = (char)('A' + rowIndex); // A, B, C, ...

                SeatTypes type;
                if (rowIndex == dto.Rows - 1)
                    type = SeatTypes.VIP;
                else if (rowIndex == dto.Rows - 2)
                    type = SeatTypes.DOUBLE;
                else
                    type = SeatTypes.STANDARD;

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

            return true;
        }
    }
}
