using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Employee;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_Management_System.Tests.Services
{
    public class DeleteScreeningRoomServiceTests
    {
        private CinemaDbContext _dbContext;
        private DeleteScreeningRoomService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase("Test_DeleteScreeningRoom_" + Guid.NewGuid())
                .Options;

            _dbContext = new CinemaDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _service = new DeleteScreeningRoomService(_dbContext);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllRooms()
        {
            _dbContext.ScreeningRooms.Add(new ScreeningRoom
            {
                Name = "Room A",
                Format = ScreenFormats.TwoD,
                Rows = 5,
                SeatsPerRow = 5,
                NumberOfSeats = 25
            });
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Room A", result[0].Name);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteRoom_WithoutFutureScreening()
        {
            var room = new ScreeningRoom
            {
                Name = "ToDelete",
                Format = ScreenFormats.TwoD,
                Rows = 4,
                SeatsPerRow = 4,
                NumberOfSeats = 16
            };

            await _dbContext.ScreeningRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            var result = await _service.DeleteAsync(new List<int> { room.Id });

            Assert.That(result.DeletedIds.Contains(room.Id));
            Assert.That(result.BlockedIds.Count == 0);
            Assert.That(await _dbContext.ScreeningRooms.CountAsync() == 0);
        }

        [Test]
        public async Task DeleteAsync_ShouldBlockRoom_WithFutureScreening()
        {
            var room = new ScreeningRoom
            {
                Name = "BlockedRoom",
                Format = ScreenFormats.ThreeD,
                Rows = 4,
                SeatsPerRow = 4,
                NumberOfSeats = 16
            };

            await _dbContext.ScreeningRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            var movie = new Movie
            {
                Title = "Test Movie",
                Description = "Desc",
                MovieLength = 90,
                AgeCategory = AgeCategory.General,
                ImagePath = "/poster.jpg"
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.SaveChangesAsync();

            _dbContext.Screenings.Add(new Screening
            {
                MovieId = movie.Id,
                ScreeningRoomId = room.Id,
                DateStartTime = DateTime.Now.AddHours(1),
                DateEndTime = DateTime.Now.AddHours(2),
                BasePrice = 20,
                Movie = movie,
                ScreeningRoom = room
            });

            await _dbContext.SaveChangesAsync();

            var result = await _service.DeleteAsync(new List<int> { room.Id });

            Assert.That(result.DeletedIds.Count == 0);
            Assert.That(result.BlockedIds.Contains(room.Id));
            Assert.That(await _dbContext.ScreeningRooms.AnyAsync(r => r.Id == room.Id));
        }
    }
}
