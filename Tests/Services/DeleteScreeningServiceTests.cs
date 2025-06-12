using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Employee;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_Management_System.Tests.Services
{
    public class DeleteScreeningServiceTests
    {
        private CinemaDbContext _dbContext;
        private DeleteScreeningService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase("Test_DeleteScreening_" + Guid.NewGuid())
                .Options;

            _dbContext = new CinemaDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _service = new DeleteScreeningService(_dbContext);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllScreenings()
        {
            var movie = new Movie
            {
                Title = "Movie A",
                Description = "desc",
                MovieLength = 100,
                AgeCategory = AgeCategory.General,
                ImagePath = "/poster.png"
            };

            var room = new ScreeningRoom
            {
                Name = "Room A",
                Format = ScreenFormats.TwoD,
                Rows = 5,
                SeatsPerRow = 5,
                NumberOfSeats = 25
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.ScreeningRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            _dbContext.Screenings.Add(new Screening
            {
                Movie = movie,
                ScreeningRoom = room,
                MovieId = movie.Id,
                ScreeningRoomId = room.Id,
                DateStartTime = DateTime.Now.AddHours(1),
                DateEndTime = DateTime.Now.AddHours(3),
                BasePrice = 25
            });

            await _dbContext.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Movie A", result[0].MovieTitle);
            Assert.AreEqual("Room A", result[0].RoomName);
        }

        [Test]
        public async Task DeleteAsync_ShouldDelete_WhenNoTickets()
        {
            var screening = await AddScreeningWithDependenciesAsync(hasTickets: false);

            var (deleted, blocked) = await _service.DeleteAsync(new List<int> { screening.Id });

            Assert.Contains(screening.Id, deleted);
            Assert.IsEmpty(blocked);
            Assert.That(await _dbContext.Screenings.CountAsync() == 0);
        }

        [Test]
        public async Task DeleteAsync_ShouldBlock_WhenHasTickets()
        {
            var screening = await AddScreeningWithDependenciesAsync(hasTickets: true);

            var (deleted, blocked) = await _service.DeleteAsync(new List<int> { screening.Id });

            Assert.IsEmpty(deleted);
            Assert.Contains(screening.Id, blocked);
            Assert.That(await _dbContext.Screenings.CountAsync() == 1);
        }

        private async Task<Screening> AddScreeningWithDependenciesAsync(bool hasTickets)
        {
            var movie = new Movie
            {
                Title = "Blockbuster",
                Description = "desc",
                MovieLength = 100,
                AgeCategory = AgeCategory.General,
                ImagePath = "/poster.png"
            };

            var room = new ScreeningRoom
            {
                Name = "Room B",
                Format = ScreenFormats.TwoD,
                Rows = 5,
                SeatsPerRow = 5,
                NumberOfSeats = 25
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.ScreeningRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            var seat = new Seat
            {
                ScreeningRoomId = room.Id,
                Row = "A",
                SeatInRow = 1,
                SeatType = SeatTypes.STANDARD
            };

            await _dbContext.Seats.AddAsync(seat);
            await _dbContext.SaveChangesAsync();

            var user = new ApplicationUser
            {
                Id = "user-123",
                Email = "test@example.com",
                UserName = "testuser"
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var screening = new Screening
            {
                MovieId = movie.Id,
                ScreeningRoomId = room.Id,
                DateStartTime = DateTime.Now.AddHours(1),
                DateEndTime = DateTime.Now.AddHours(3),
                BasePrice = 20,
                Movie = movie,
                ScreeningRoom = room
            };

            await _dbContext.Screenings.AddAsync(screening);
            await _dbContext.SaveChangesAsync();

            if (hasTickets)
            {
                var ticket = new Ticket
                {
                    ScreeningId = screening.Id,
                    SeatId = seat.Id,
                    UserId = user.Id,
                    PurchaseDate = DateTime.Now,
                    FinalPrice = 20
                };

                await _dbContext.Tickets.AddAsync(ticket);
                await _dbContext.SaveChangesAsync();
            }

            return screening;
        }

    }
}
