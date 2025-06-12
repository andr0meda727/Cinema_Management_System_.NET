using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Employee;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Cinema_Management_System.Tests.Services
{
    public class AddScreeningServiceTests
    {
        private CinemaDbContext _dbContext;
        private AddScreeningService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase("Test_AddScreening")
                .Options;

            _dbContext = new CinemaDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _service = new AddScreeningService(_dbContext);
        }

        [Test]
        public async Task AddAsync_ShouldAddScreening_WhenValid()
        {
            var movie = new Movie
            {
                Title = "Test",
                Description = "Test description",
                MovieLength = 100,
                AgeCategory = AgeCategory.General,
                ImagePath = "/uploads/test.jpg"
            };

            var room = new ScreeningRoom
            {
                Name = "Sala 1",
                Format = ScreenFormats.TwoD,
                Rows = 5,
                SeatsPerRow = 10,
                NumberOfSeats = 50
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.ScreeningRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            var dto = new AddScreeningDTO
            {
                MovieId = movie.Id,
                ScreeningRoomId = room.Id,
                StartTime = DateTime.Now.AddHours(2),
                BasePrice = 20
            };

            var (success, error) = await _service.AddAsync(dto);

            Assert.IsTrue(success);
            Assert.IsNull(error);
            Assert.AreEqual(1, await _dbContext.Screenings.CountAsync());
        }

        [Test]
        public async Task AddAsync_ShouldReturnError_WhenConflict()
        {
            var movie = new Movie
            {
                Title = "Test",
                Description = "Test conflict description",
                MovieLength = 90,
                AgeCategory = AgeCategory.SevenPlus,
                ImagePath = "/uploads/conflict.jpg"
            };

            var room = new ScreeningRoom
            {
                Name = "Sala 1",
                Format = ScreenFormats.ThreeD,
                Rows = 4,
                SeatsPerRow = 4,
                NumberOfSeats = 16
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.ScreeningRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            var screening = new Screening
            {
                MovieId = movie.Id,
                ScreeningRoomId = room.Id,
                DateStartTime = DateTime.Today.AddHours(18),
                DateEndTime = DateTime.Today.AddHours(20),
                BasePrice = 15,
                Movie = movie,
                ScreeningRoom = room
            };

            _dbContext.Screenings.Add(screening);
            await _dbContext.SaveChangesAsync();

            var dto = new AddScreeningDTO
            {
                MovieId = movie.Id,
                ScreeningRoomId = room.Id,
                StartTime = DateTime.Today.AddHours(19), // conflict
                BasePrice = 20
            };

            var (success, errorMsg) = await _service.AddAsync(dto);

            Assert.IsFalse(success);
            Assert.AreEqual("Konflikt z instniejącym seansem o tej godzinie.", errorMsg);
        }

    }
}
