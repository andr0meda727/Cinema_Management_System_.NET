using Cinema_Management_System.DTOs.Auth;

namespace Cinema_Management_System.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResult> LoginAsync(LoginDTO loginDTO);
    }
}
