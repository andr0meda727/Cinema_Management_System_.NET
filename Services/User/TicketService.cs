using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Mappers;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Helpers;
using Cinema_Management_System.Services.Interfaces;
using Cinema_Management_System.Services.PDF;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Cinema_Management_System.Services.User
{
    public class TicketService : ITicketService
    {
        private readonly CinemaDbContext _context;
        private readonly SeatSelectionMapper _mapper;
        private readonly TicketMapper _ticketMapper;
        private readonly ITicketPdfService _ticketPdfService;
        private readonly IEmailService _emailService;


        public TicketService(CinemaDbContext context, SeatSelectionMapper mapper, TicketMapper ticketMapper, ITicketPdfService pdfService,IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _ticketMapper = ticketMapper;
            _ticketPdfService = pdfService;
            _emailService = emailService;
        }

        public async Task<PurchaseResult> PurchaseTicketsAsync(BuyTicketDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var screening = await _context.Screenings
                    .Include(s => s.Movie)
                    .FirstOrDefaultAsync(s => s.Id == dto.ScreeningId);

                if (screening == null)
                {
                    return PurchaseResult.CreateFailureResult("Screening not found");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                if (user == null)
                {
                    return PurchaseResult.CreateFailureResult("User not found");
                }

                var seats = await _context.Seats
                    .Include(s => s.ScreeningRoom)
                    .Where(s => dto.SeatIds.Contains(s.Id) && s.ScreeningRoomId == screening.ScreeningRoomId)
                    .ToListAsync();

                var takenSeatIds = await _context.Tickets
                    .Where(t => t.ScreeningId == dto.ScreeningId && dto.SeatIds.Contains(t.SeatId))
                    .Select(t => t.SeatId)
                    .ToListAsync();

                if (takenSeatIds.Any())
                {
                    return PurchaseResult.CreateFailureResult($"Seats {string.Join(", ", takenSeatIds)} are already taken");
                }


                if (seats.Count != dto.SeatIds.Count)
                {
                    var missingSeats = dto.SeatIds.Except(seats.Select(s => s.Id)).ToList();
                    return PurchaseResult.CreateFailureResult($"Seats {string.Join(", ", missingSeats)} not found");
                }

                var tickets = new List<Ticket>();
                foreach (var seat in seats)
                {
                    var ticket = new Ticket
                    {
                        ScreeningId = dto.ScreeningId,
                        SeatId = seat.Id,
                        UserId = dto.UserId,
                        FinalPrice = screening.BasePrice * TicketPricingHelper.GetSeatTypeMultiplier(seat.SeatType),
                        PurchaseDate = DateTime.Now
                    };

                    tickets.Add(ticket);
                }

                await _context.Tickets.AddRangeAsync(tickets);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                var pdfStreams = new List<MemoryStream>();
                foreach (var ticket in tickets)
                {
                    var ticketDto = _ticketMapper.TicketToTicketDetailedDTO(ticket);
                    var pdfStream = _ticketPdfService.GeneratePdf(ticketDto);
                    pdfStreams.Add(pdfStream);
                }

                var userEmail = user.Email;

                var subject = "AbsoluteCinema - bilety";
                var body = "<h1>Dziękujemy za zakup!</h1><p>W załącznikach znajdziesz swoje bilety.</p>";

                var attachments = new List<Attachment>();
                for (int i = 0; i < pdfStreams.Count; i++)
                {
                    var pdfStream = pdfStreams[i];
                    var attachment = new Attachment(pdfStream, $"Bilet_{i + 1}.pdf", "application/pdf");
                    attachments.Add(attachment);
                }

                await _emailService.SendEmailWithAttachmentsAsync(userEmail, subject, body, attachments);

                foreach (var stream in pdfStreams)
                {
                    stream.Dispose();
                }

                return PurchaseResult.CreateSuccessResult(tickets.Select(t => t.Id).ToList());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // logger
                return PurchaseResult.CreateFailureResult("Error processing your purchase");
            }
        }

        public async Task<SeatSelectionDTO?> GetSeatSelectionAsync(int screeningId)
        {
            var screening = await _context.Screenings
                .Include(s => s.ScreeningRoom)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening == null) return null;

            var seats = await _context.Seats
                .Where(s => s.ScreeningRoomId == screening.ScreeningRoomId)
                .ToListAsync();

            var takenSeatIds = screening.Tickets
                .Select(t => t.SeatId)
                .ToList();

            var dto = _mapper.ScreeningToSeatSelectionDTO(screening, seats);

            foreach (var seat in dto.Seats)
            {
                seat.isTaken = takenSeatIds.Contains(seat.Id);
                seat.Price = screening.BasePrice * TicketPricingHelper.GetSeatTypeMultiplier(seat.SeatType);
            }

            return dto;
        }

        public async Task<List<BasicTicketDTO>> GetUserBasicTicketsAsync(string userId)
        {
            var user = _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .OrderBy(t => t.Screening.DateStartTime)
                .ToListAsync();

            return tickets.Select(t => _ticketMapper.TicketToTicketBasicDTO(t)).ToList();
        }
        
        public async Task<DetailedTicketDTO> GetUserDetailedTicketAsync(string userId, int ticketId)
        {
            var user = _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var ticket = await _context.Tickets
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.ScreeningRoom)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.UserId == userId);

            return ticket == null ? null : _ticketMapper.TicketToTicketDetailedDTO(ticket);
        }

        public async Task<List<DetailedTicketDTO>> GetTicketSummariesAsync(string userId, List<int> ticketIds)
        {
            return await _context.Tickets
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.ScreeningRoom)
                .Where(t => ticketIds.Contains(t.Id) && t.UserId == userId)
                .Select(t => _ticketMapper.TicketToTicketDetailedDTO(t))
                .ToListAsync();
        }

        public async Task<bool> RefundTicketAsync(string userId, int ticketId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Screening)
                    .FirstOrDefaultAsync(t => t.Id == ticketId && t.UserId == userId);

                if (ticket == null) return false;

                if (ticket.Screening.DateStartTime <= DateTime.Now)
                {
                    return false;
                }

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
