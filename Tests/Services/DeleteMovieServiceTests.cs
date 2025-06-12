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
    public class DeleteMovieServiceTests
    {
        private CinemaDbContext _dbContext;
        private DeleteMovieService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase("Test_DeleteMovie_" + Guid.NewGuid())
                .Options;

            _dbContext = new CinemaDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _service = new DeleteMovieService(_dbContext);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllMovies()
        {
            _dbContext.Movies.Add(new Movie
            {
                Title = "Test Movie",
                Description = "Desc",
                MovieLength = 90,
                AgeCategory = AgeCategory.General,
                ImagePath = "/path.jpg"
            });
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Movie", result[0].Title);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteMovie_WithoutFutureScreening()
        {
            var movie = new Movie
            {
                Title = "Old",
                Description = "Old desc",
                MovieLength = 90,
                AgeCategory = AgeCategory.General,
                ImagePath = "/path.jpg"
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.SaveChangesAsync();

            var result = await _service.DeleteAsync(new List<int> { movie.Id });

            Assert.That(result.DeletedIds.Contains(movie.Id));
            Assert.That(result.BlockedIds.Count == 0);
            Assert.That(await _dbContext.Movies.CountAsync() == 0);
        }

        [Test]
        public async Task DeleteAsync_ShouldBlockMovie_WithFutureScreening()
        {
            var movie = new Movie
            {
                Title = "Future",
                Description = "Desc",
                MovieLength = 90,
                AgeCategory = AgeCategory.General,
                ImagePath = "/path.jpg"
            };

            var screeningRoom = new ScreeningRoom
            {
                Name = "Room 1",
                Format = ScreenFormats.TwoD,
                Rows = 5,
                SeatsPerRow = 5,
                NumberOfSeats = 25
            };

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.ScreeningRooms.AddAsync(screeningRoom);
            await _dbContext.SaveChangesAsync();

            _dbContext.Screenings.Add(new Screening
            {
                MovieId = movie.Id,
                ScreeningRoomId = screeningRoom.Id,
                DateStartTime = DateTime.Now.AddHours(1),
                DateEndTime = DateTime.Now.AddHours(2),
                BasePrice = 15,
                Movie = movie,
                ScreeningRoom = screeningRoom
            });
            await _dbContext.SaveChangesAsync();

            var result = await _service.DeleteAsync(new List<int> { movie.Id });

            Assert.That(result.DeletedIds.Count == 0);
            Assert.That(result.BlockedIds.Contains(movie.Id));
            Assert.That(await _dbContext.Movies.AnyAsync(m => m.Id == movie.Id));
        }
    }
}
