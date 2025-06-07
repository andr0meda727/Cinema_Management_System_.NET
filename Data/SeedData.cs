using Cinema_Management_System.Models.Users;
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

            // Ensure DB is created
            context.Database.Migrate();

            // Seed roles if not exists
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role {Name = "User" },
                    new Role {Name = "Employee" },
                    new Role {Name = "Admin" }
                );
                await context.SaveChangesAsync();
            }
            // 2. Pobierz rolę "Admin"
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

            // Create admin user if not exists
            // 3. Dodaj admina jeśli nie istnieje
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,
                    RoleId = adminRole.Id // <-- już istniejące ID
                };

                await userManager.CreateAsync(admin, "zaq1@WSX");
            }
        }

    }
}
