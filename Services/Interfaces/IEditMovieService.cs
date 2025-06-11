using Cinema_Management_System.DTOs.Employee;
using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IEditMovieService
    {
        Task<List<Movie>> GetAllMoviesAsync();
        Task<EditMovieDTO?> GetByIdAsync(int id);
        Task<bool> UpdateMovieAsync(int id, EditMovieDTO dto);
    }
}
