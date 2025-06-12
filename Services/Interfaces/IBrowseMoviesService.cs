using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IBrowseMoviesService
    {
        Task<List<Movie>> GetAllMoviesAsync();
    }
}
