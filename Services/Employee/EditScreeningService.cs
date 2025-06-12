using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.DTOs.Screening;
using Cinema_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class EditScreeningService : IEditScreeningService
{
    private readonly CinemaDbContext _db;

    public EditScreeningService(CinemaDbContext db)
    {
        _db = db;
    }

    public async Task<List<DetailedScreeningDTO>> GetAllAsync()
    {
        return await _db.Screenings
            .Include(s => s.Movie)
            .Include(s => s.ScreeningRoom)
            .Select(s => new DetailedScreeningDTO
            {
                Id = s.Id,
                Title = s.Movie.Title,
                DateStartTime = s.DateStartTime,
                DateEndTime = s.DateEndTime,
                ShortDescription = s.Movie.Description.Length > 100
                    ? s.Movie.Description.Substring(0, 100) + "..."
                    : s.Movie.Description,
                MovieDescription = s.Movie.Description,
                AgeCategory = s.Movie.AgeCategory,
                MoviePosterUrl = s.Movie.ImagePath,
                ScreeningRoomName = s.ScreeningRoom.Name,
                ScreeningRoomFormat = s.ScreeningRoom.Format,
                BasePrice = s.BasePrice
            })
            .ToListAsync();
    }

    public async Task<EditScreeningDTO?> GetByIdAsync(int id)
    {
        var screening = await _db.Screenings.FindAsync(id);
        if (screening == null)
            return null;

        var hasTickets = await _db.Tickets.AnyAsync(t => t.ScreeningId == id);
        if (hasTickets)
            return null;

        return new EditScreeningDTO
        {
            Id = screening.Id,
            MovieId = screening.MovieId,
            ScreeningRoomId = screening.ScreeningRoomId,
            DateStartTime = screening.DateStartTime,
            BasePrice = screening.BasePrice
        };
    }

    public async Task<bool> UpdateAsync(EditScreeningDTO dto)
    {
        Console.WriteLine($"Start: {dto.DateStartTime}, MovieId: {dto.MovieId}, ScreeningRoomId: {dto.ScreeningRoomId}");

        var screening = await _db.Screenings
            .Include(s => s.Tickets)
            .FirstOrDefaultAsync(s => s.Id == dto.Id);

        if (screening == null || screening.Tickets.Any())
            return false;

        var movie = await _db.Movies.FindAsync(dto.MovieId);
        if (movie == null)
            return false;

        var proposedStart = dto.DateStartTime;
        var proposedEnd = proposedStart.AddMinutes(movie.MovieLength);

        if (proposedStart < DateTime.Now)
            return false;

        var hasConflict = await _db.Screenings
            .Where(s => s.Id != dto.Id && s.ScreeningRoomId == dto.ScreeningRoomId)
            .AnyAsync(s =>
                (proposedStart < s.DateEndTime && proposedEnd > s.DateStartTime)
            );

        if (hasConflict)
            return false;

        screening.DateStartTime = proposedStart;
        screening.DateEndTime = proposedEnd;
        screening.BasePrice = dto.BasePrice;

        await _db.SaveChangesAsync();
        return true;
    }

}
