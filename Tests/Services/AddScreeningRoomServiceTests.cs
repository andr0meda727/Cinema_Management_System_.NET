using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Cinema_Management_System.Data;
using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.DTOs.Employee;

namespace Tests
{
    [TestFixture]
    public class AddScreeningRoomServiceTests
    {
        private CinemaDbContext _dbContext;
        private AddScreeningRoomService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase("Test_AddScreeningRoom")
                .Options;

            _dbContext = new CinemaDbContext(options);
            _dbContext.Database.EnsureDeleted(); // optional reset
            _dbContext.Database.EnsureCreated();

            _service = new AddScreeningRoomService(_dbContext);
        }

        [Test]
        public async Task AddAsync_ShouldAddRoom_WhenValid()
        {
            var dto = new CreateScreeningRoomDTO
            {
                Name = "Sala 1",
                Format = Cinema_Management_System.Models.Cinema.ScreenFormats.TwoD,
                Rows = 5,
                SeatsPerRow = 10
            };

            var result = await _service.AddAsync(dto);

            Assert.IsTrue(result);
            Assert.AreEqual(1, await _dbContext.ScreeningRooms.CountAsync());
            Assert.AreEqual(50, await _dbContext.Seats.CountAsync());
        }
    }
}
