using Cinema_Management_System.Data;
using Cinema_Management_System.Mappers;
using Cinema_Management_System.Models.Users;
using Cinema_Management_System.Services.Auth;
using Cinema_Management_System.Services.Em;
using Cinema_Management_System.Services.Employee;
using Cinema_Management_System.Services.Interfaces;
using Cinema_Management_System.Services.PDF;
using Cinema_Management_System.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NLog;
using NLog.Web;

namespace Cinema_Management_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = LogManager.Setup()
                .LoadConfigurationFromFile("nlog.config", optional: false)
                .GetCurrentClassLogger();

            try
            {
                logger.Info("Application is starting...");
                if (!Directory.Exists("logs"))
                    Directory.CreateDirectory("logs");
                logger.Warn("NLog dzia³a — testowy wpis");
                var builder = WebApplication.CreateBuilder(args);
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                builder.Host.UseNLog();

                builder.Services.AddRazorPages();

                builder.Services.AddDbContext<CinemaDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddScoped<ScreeningMapper>();
                builder.Services.AddScoped<TicketMapper>();
                builder.Services.AddScoped<SeatSelectionMapper>();

                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<ICookieService, CookieService>();

                builder.Services.AddScoped<IScreeningService, ScreeningService>();
                builder.Services.AddScoped<ITicketService, TicketService>();
                builder.Services.AddScoped<ITicketPdfService, TicketPdfService>();
                builder.Services.AddScoped<IEmailService, EmailService>();

                builder.Services.AddScoped<IAddMovieService, AddMovieService>();
                builder.Services.AddScoped<IDeleteMovieService, DeleteMovieService>();
                builder.Services.AddScoped<IBrowseMoviesService, BrowseMoviesService>();
                builder.Services.AddScoped<IEditMovieService, EditMovieService>();

                builder.Services.AddScoped<IAddScreeningRoomService, AddScreeningRoomService>();
                builder.Services.AddScoped<IDeleteScreeningRoomService, DeleteScreeningRoomService>();
                builder.Services.AddScoped<IBrowseScreeningRoomService, BrowseScreeningRoomService>();
                builder.Services.AddScoped<IEditScreeningRoomService, EditScreeningRoomService>();

                builder.Services.AddScoped<IAddScreeningService, AddScreeningService>();
                builder.Services.AddScoped<IDeleteScreeningService, DeleteScreeningService>();
                builder.Services.AddScoped<IBrowseScreeningService, BrowseScreeningService>();
                builder.Services.AddScoped<IEditScreeningService, EditScreeningService>();

                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                }).AddEntityFrameworkStores<CinemaDbContext>()
                  .AddDefaultTokenProviders();

                var authenticationSettings = new AuthenticationSettings();
                builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
                builder.Services.AddSingleton(authenticationSettings);

                builder.Services.AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = authenticationSettings.JwtIssuer,
                        ValidAudience = authenticationSettings.JwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("jwt"))
                            {
                                context.Token = context.Request.Cookies["jwt"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

                builder.Services.AddControllers(); // dla AuthApiController

                var app = builder.Build();

           
                using (var scope = app.Services.CreateScope())
                {
                    await SeedData.InitializeAsync(scope.ServiceProvider);
                }

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

                app.MapControllers(); 
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();
                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application stopped due to exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
