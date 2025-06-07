using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services
{
    public class ScreeningService
    {
        private readonly CinemaDbContext _context;
        private readonly ScreeningMapper _mapper;

        public ScreeningService(CinemaDbContext context, ScreeningMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BasicScreeningDTO>> GetUpcomingScreeningsAsync(int daysAhead = 7)
        {
            var fromDate = DateTime.Now;
            var toDate = fromDate.AddDays(daysAhead);

            var screenings = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom)
                .Where(s => s.DateStartTime >= fromDate && s.DateStartTime <= toDate)
                .OrderBy(s => s.DateStartTime)
                .ToListAsync();

            return screenings.Select(_mapper.ScreeningToBasicScreeningDTOWithShortDesc).ToList();
        }
    }
}
