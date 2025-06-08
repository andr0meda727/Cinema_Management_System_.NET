using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Seat;
using Cinema_Management_System.Mappers;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cinema_Management_System.Services
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

        public async Task<Ticket> CreateTicketAsync(int screeningId, int seatId, string userId)
        {
            var screening = await _context.Screenings.FirstOrDefaultAsync(s => s.Id == screeningId);
            if (screening == null)
            {
                throw new Exception("Invalid screening");
            }

            var seat = await _context.Seats.FirstOrDefaultAsync(s => s.Id == seatId);
            if (seat == null)
            {
                throw new Exception("Invalid seat");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("Invalid user");
            }

            decimal finalPrice = Math.Round(TicketPricingHelper.GetSeatTypeMultiplier(seat.SeatType) * screening.BasePrice, 2);

            var ticket = new Ticket
            {
                ScreeningId = screeningId,
                SeatId = seatId,
                UserId = userId,
                FinalPrice = finalPrice,
                Screening = screening,
                Seat = seat,
                User = user
            };


            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task<SeatSelectionDTO?> GetSeatSelectionAsync(int screeningId)
        {
            var screening = await _context.Screenings
                .Include(s => s.ScreeningRoom)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == 4);

            if (screening == null) return null;

            var seats = await _context.Seats
                .Where(s => s.ScreeningRoomId == 3)
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
