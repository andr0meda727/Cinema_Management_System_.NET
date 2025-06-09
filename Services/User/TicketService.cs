using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Mappers;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cinema_Management_System.Services.User
{
    public class TicketService
    {
        private readonly CinemaDbContext _context;
        private readonly SeatSelectionMapper _mapper;

        public TicketService(CinemaDbContext context, SeatSelectionMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public class PurchaseResult
        {
            public bool Success { get; }
            public string Message { get; }
            public int? OrderId { get; }

            private PurchaseResult(bool success, string message, int? orderId)
            {
                Success = success;
                Message = message;
                OrderId = orderId;
            }

            public static PurchaseResult SuccessResult(int orderId) => new(true, string.Empty, orderId);
            public static PurchaseResult Failure(string message) => new(false, message, null);
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
                    .Where(s => dto.SeatIds.Contains(s.Id) &&
                               s.ScreeningRoomId == screening.ScreeningRoomId)
                    .ToListAsync();

                if (seats.Count != dto.SeatIds.Count)
                {
                    return PurchaseResult.Failure("Some seats not found");
                }

                if (seats.Any(s => s.SeatStatus))
                {
                    return PurchaseResult.Failure("Some seats are already taken, choose again");
                }

                var tickets = new List<Ticket>();
                foreach (var seat in seats)
                {
                    var ticket = new Ticket
                    {
                        ScreeningId = dto.ScreeningId,
                        Screening = screening,
                        SeatId = seat.Id,
                        Seat = seat,
                        UserId = dto.UserId,
                        User = user,
                        FinalPrice = screening.BasePrice * TicketPricingHelper.GetSeatTypeMultiplier(seat.SeatType),
                        PurchaseDate = DateTime.UtcNow
                    };

                    tickets.Add(ticket);
                    seat.SeatStatus = true;
                    seat.Ticket = ticket;
                }

                await _context.Tickets.AddRangeAsync(tickets);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return PurchaseResult.SuccessResult(tickets.First().Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

        //    public async Task<List<BasicTicketDto>> GetUserTickets(int userId)
        //    {
        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //        if (user == null)
        //        {
        //            throw new Exception("Invalid user");
        //        }

        //    }
    }
}
