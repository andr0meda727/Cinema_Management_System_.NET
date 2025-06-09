using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Mappers;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cinema_Management_System.Services.User
{
    public class TicketService
    {
        private readonly CinemaDbContext _context;
        private readonly SeatSelectionMapper _mapper;
        private readonly TicketMapper _ticketMapper;


        public TicketService(CinemaDbContext context, SeatSelectionMapper mapper, TicketMapper ticketMapper)
        {
            _context = context;
            _mapper = mapper;
            _ticketMapper = ticketMapper;
        }

        public class PurchaseResult
        {
            public bool Success { get; }
            public string Message { get; }
            public List<int> TicketIds { get; }
            public Guid? OrderId { get; }

            private PurchaseResult(bool success, string message, List<int> ticketIds, int? orderId = null)
            {
                Success = success;
                Message = message;
                TicketIds = ticketIds;
                OrderId = Guid.NewGuid();
            }


            public static PurchaseResult SuccessResult(List<int> ticketIds, int? orderId = null)
                => new(true, string.Empty, ticketIds, orderId);

            public static PurchaseResult Failure(string message)
                => new(false, message, null);
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
                    return PurchaseResult.Failure("Screening not found");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                if (user == null)
                {
                    return PurchaseResult.Failure("User not found");
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
                    return PurchaseResult.Failure($"Seats {string.Join(", ", takenSeatIds)} are already taken");
                }


                if (seats.Count != dto.SeatIds.Count)
                {
                    var missingSeats = dto.SeatIds.Except(seats.Select(s => s.Id)).ToList();
                    return PurchaseResult.Failure($"Seats {string.Join(", ", missingSeats)} not found");
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

                return PurchaseResult.SuccessResult(tickets.Select(t => t.Id).ToList());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // logger
                return PurchaseResult.Failure("Error processing your purchase");
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

        public async Task<List<DetailedTicketDTO>> GetTicketSummariesAsync(List<int> ticketIds)
        {
            return await _context.Tickets
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.ScreeningRoom)
                .Where(t => ticketIds.Contains(t.Id))
                .Select(t => _ticketMapper.TicketToTicketDetailedDTO(t))
                .ToListAsync();
        }
    }
}
