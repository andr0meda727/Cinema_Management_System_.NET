using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Employee
{
    public class ScreeningRoomService
    {
        private readonly CinemaDbContext _db;

        public ScreeningRoomService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(CreateScreeningRoomDTO dto)
        {
   
                // Oblicz liczbę miejsc
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
                await _db.SaveChangesAsync(); // Potrzebne do uzyskania room.Id

                // Tworzenie miejsc
                var seats = new List<Seat>();
                for (int rowIndex = 0; rowIndex < dto.Rows; rowIndex++)
                {
                    char rowLetter = (char)('A' + rowIndex); // A, B, C, ...
                    for (int seatNum = 1; seatNum <= dto.SeatsPerRow; seatNum++)
                    {
                        seats.Add(new Seat
                        {
                            ScreeningRoomId = room.Id,
                            Row = rowLetter.ToString(),
                            SeatInRow = seatNum,
                            SeatType = SeatTypes.STANDARD,
                            SeatStatus = false
                        });
                    }
                }

                _db.Seats.AddRange(seats);
                await _db.SaveChangesAsync();

                return true;
            
           
        }

    }
}
