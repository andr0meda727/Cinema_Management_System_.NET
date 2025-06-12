using Cinema_Management_System.Data;
using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Services.Employee;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_System.Tests.Services
{
    public class AddMovieServiceTests
    {
        private CinemaDbContext _dbContext;
        private AddMovieService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase("Test_AddMovie")
                .Options;

            _dbContext = new CinemaDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _service = new AddMovieService(_dbContext);
        }

        [Test]
        public async Task AddAsync_ShouldAddMovie_WhenValid()
        {
            // Tworzenie "pliku" w pamięci
            var content = "fake image content";
            var fileName = "poster.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var formFile = new FormFile(ms, 0, ms.Length, "PosterFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var dto = new AddMovieDTO
            {
                Title = "Inception",
                Description = "Mind-bending thriller",
                MovieLength = 148,
                AgeCategory = AgeCategory.SixteenPlus,
                PosterFile = formFile
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, await _dbContext.Movies.CountAsync());
        }

        [Test]
        public async Task AddAsync_ShouldFail_WhenTitleAlreadyExists()
        {
            var existing = new Movie
            {
                Title = "Matrix",
                Description = "Original",
                MovieLength = 120,
                AgeCategory = AgeCategory.TwelvePlus,
                ImagePath = "/uploads/old.jpg"
            };

            _dbContext.Movies.Add(existing);
            await _dbContext.SaveChangesAsync();

            var dto = new AddMovieDTO
            {
                Title = "Matrix", // taki sam tytuł
                Description = "Duplicate",
                MovieLength = 100,
                AgeCategory = AgeCategory.General,
                PosterFile = null
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, await _dbContext.Movies.CountAsync());
        }
    }
}
