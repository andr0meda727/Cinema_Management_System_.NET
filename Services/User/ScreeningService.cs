using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Services.User
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

        public async Task<List<BasicScreeningDTO>> GetScreeningsAsyncDate(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            var screenings = await _context.Screenings
                .Include(s => s.Movie)
                .Where(s => s.DateStartTime >= startOfDay && s.DateStartTime <= endOfDay)
                .OrderBy(s => s.DateStartTime)
                .ToListAsync();

            return screenings.Select(_mapper.ScreeningToBasicScreeningDTOWithShortDesc).ToList();
        }

        public async Task<DetailedScreeningDTO?> GetDetailedScreeningByIdAsync(int screeningId)
        {
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom)
                .Where(s => s.Id == screeningId)
                .Select(s => _mapper.ScreeningToScreeningDetailedDTO(s))
                .FirstOrDefaultAsync();

            if (screening == null) return null;

            return screening;
        }
    }
}
