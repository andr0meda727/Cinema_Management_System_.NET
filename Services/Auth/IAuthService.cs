using Cinema_Management_System.DTOs.Auth;

namespace Cinema_Management_System.Services.Auth
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(RegisterDTO registerDTO);
        Task<string?> LoginAsync(LoginDTO loginDTO);
    }

    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
