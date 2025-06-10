using Cinema_Management_System.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterUserAsync(RegisterDTO registerDTO);
        Task<string?> LoginAsync(LoginDTO loginDTO);
        Task<IdentityResult> RegisterEmployeeAsync(RegisterDTO model);

    }

    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
