using Cinema_Management_System.Models.Cinema;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure DB is created
            await context.Database.MigrateAsync();

            // Seed roles
            string[] roles = ["User", "Employee", "Admin"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@cinema.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(admin, "zaq1@WSX");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed test user
            if (await userManager.FindByNameAsync("testuser") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "testuser",
                    Email = "user@cinema.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };
                await userManager.CreateAsync(user, "Test123!");
                await userManager.AddToRoleAsync(user, "User");
            }

            // Only seed if database is empty
            if (!context.Movies.Any())
            {
                // Seed Movies
                var movies = new List<Movie>
                {
                    new() {
                        Title = "Dune: Part Two",
                        Description = "Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family.",
                        MovieLength = 166,
                        AgeCategory = AgeCategory.TwelvePlus,
                        ImagePath = "dune2.jpg"
                    },
                    new() {
                        Title = "The Batman",
                        Description = "When a sadistic serial killer begins murdering key political figures in Gotham, Batman is forced to investigate.",
                        MovieLength = 176,
                        AgeCategory = AgeCategory.SixteenPlus,
                        ImagePath = "batman.jpg"
                    },
                    new() {
                        Title = "Elemental",
                        Description = "In a city where fire, water, land and air residents live together, a fiery young woman and a go-with-the-flow guy discover something elemental: how much they actually have in common.",
                        MovieLength = 102,
                        AgeCategory = AgeCategory.General,
                        ImagePath = "elemental.jpg"
                    }
                };
                await context.Movies.AddRangeAsync(movies);
                await context.SaveChangesAsync();

                // Seed Screening Rooms
                var rooms = new List<ScreeningRoom>
                {
                    new() {
                        Name = "Sala 1",
                        Format = ScreenFormats.TwoD,
                        Rows = 10,
                        SeatsPerRow = 15,
                        NumberOfSeats = 10 * 15
                    },
                    new() {
                        Name = "Sala 2",
                        Format = ScreenFormats.ThreeD,
                        Rows = 8,
                        SeatsPerRow = 12,
                        NumberOfSeats = 8 * 12
                    },
                    new() {
                        Name = "Sala 3",
                        Format = ScreenFormats.Imax,
                        Rows = 12,
                        SeatsPerRow = 20,
                        NumberOfSeats = 12 * 20
                    }
                };
                await context.ScreeningRooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();

                // Generate seats for each room
                foreach (var room in rooms)
                {
                    var seats = new List<Seat>();
                    for (int row = 0; row < room.Rows; row++)
                    {
                        for (int seatNum = 1; seatNum < room.SeatsPerRow; seatNum++)
                        {
                            var seatType = SeatTypes.STANDARD;
                            if (row == room.Rows - 2)
                                seatType = SeatTypes.VIP;
                            if (row == room.Rows - 1)
                                seatType = SeatTypes.DOUBLE;


                            seats.Add(new Seat
                            {
                                ScreeningRoomId = room.Id,
                                ScreeningRoom = room,
                                Row = ((char)('A' + row)).ToString(),
                                SeatInRow = seatNum,
                                SeatType = seatType,
                                SeatStatus = false
                            });
                        }
                    }
                    await context.Seats.AddRangeAsync(seats);
                }
                await context.SaveChangesAsync();

                // Seed Screenings (next 7 days)
                var now = DateTime.Now;
                var screenings = new List<Screening>();
                var random = new Random();

                foreach (var movie in movies)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var daysOffset = random.Next(0, 7);
                        var hour = random.Next(10, 22);
                        var room = rooms[random.Next(rooms.Count)];

                        var startTime = now.AddDays(daysOffset).Date.AddHours(hour);
                        var endTime = startTime.AddMinutes(movie.MovieLength + 15); // +15 mins for cleaning

                        screenings.Add(new Screening
                        {
                            MovieId = movie.Id,
                            Movie = movie,
                            ScreeningRoomId = room.Id,
                            ScreeningRoom = room,
                            DateStartTime = startTime,
                            DateEndTime = endTime,
                            BasePrice = room.Format switch
                            {
                                ScreenFormats.Imax => 35.00m,
                                ScreenFormats.ThreeD => 30.00m,
                                _ => 25.00m
                            }
                        });
                    }
                }
                await context.Screenings.AddRangeAsync(screenings);
                await context.SaveChangesAsync();

                // Seed some tickets for test user
                var user = await userManager.FindByNameAsync("testuser");
                if (user != null)
                {
                    var screening = screenings.First();
                    var seats = await context.Seats
                        .Where(s => s.ScreeningRoomId == screening.ScreeningRoomId)
                        .Take(3)
                        .ToListAsync();

                    var tickets = seats.Select(seat => new Ticket
                    {
                        ScreeningId = screening.Id,
                        Screening = screening,
                        SeatId = seat.Id,
                        Seat = seat,
                        UserId = user.Id,
                        User = user,
                        FinalPrice = screening.BasePrice * TicketPricingHelper.GetSeatTypeMultiplier(seat.SeatType),
                        PurchaseDate = DateTime.UtcNow
                    }).ToList();

                    await context.Tickets.AddRangeAsync(tickets);
                    await context.SaveChangesAsync();

                    // Mark seats as occupied
                    foreach (var seat in seats)
                    {
                        seat.SeatStatus = true;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}