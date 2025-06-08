using System;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Data;
using Microsoft.EntityFrameworkCore;
using Cinema_Management_System.Services.Helpers;

namespace Cinema_Management_System.Services
{
    public class TicketService
    {
        private readonly CinemaDbContext _context;

        public TicketService(CinemaDbContext context)
        {
            _context = context;
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
