using Cinema_Management_System.Data;
using Cinema_Management_System.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Cinema_Management_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
        
            //databse for users
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //ASP.NET Identity options
            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<UserDbContext>();

            /*  var authenticationSettings = new AuthenticationSettings();

              builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

              builder.Services.AddSingleton(authenticationSettings);

              builder.Services.AddAuthentication(option =>
              {
                  option.DefaultAuthenticateScheme = "Bearer";
                  option.DefaultScheme = "Bearer";
                  option.DefaultChallengeScheme = "Bearer";
              }).AddJwtBearer(cfg =>
              {
                  cfg.RequireHttpsMetadata = false;
                  cfg.SaveToken = true;
                  cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                  {
                      ValidIssuer = authenticationSettings.JwtIssuser,
                      ValidAudience = authenticationSettings.JwtIssuser,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                  };
              });
  */
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Domyœlna trasa
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.Run();
        }
    }
}
