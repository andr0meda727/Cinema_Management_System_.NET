using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.Employee
{

    public class BrowseScreeningService
    {
        private readonly CinemaDbContext _db;

        public BrowseScreeningService(CinemaDbContext db)
        {
            _db = db;
        }

        public async Task<List<BrowseScreeningDTO>> GetScreeningsByDateAsync(DateTime date)
        {
            // Ustalamy zakres dnia: od północy do północy następnego dnia
            DateTime startDate = date.Date;
            DateTime endDate = startDate.AddDays(1);

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

            return screenings;
        }
    }
}
