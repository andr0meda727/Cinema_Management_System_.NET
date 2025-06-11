using Cinema_Management_System.DTOs.Employee;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IDeleteMovieService
    {
        Task<List<DeleteMovieDTO>> GetAllAsync();
        Task<(List<int> DeletedIds, List<int> BlockedIds)> DeleteAsync(List<int> movieIds);
    }
}
