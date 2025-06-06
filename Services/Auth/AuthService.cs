using Cinema_Management_System.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Cinema_Management_System.Models.Users;

namespace Cinema_Management_System.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = DateTime.Now,
                RoleId = 1 //  default User
            };
            var result = await _userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return new AuthResult
            {
                Succeeded = result.Succeeded,
                IdentityErrors = result.Errors
            };

        }

        public async Task<AuthResult> LoginAsync(LoginDTO model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

            return new AuthResult
            {
                Succeeded = result.Succeeded,
                IdentityErrors = result.Succeeded ? Enumerable.Empty<IdentityError>() : [new IdentityError { Description = "Invalid credentials" }]
            };
        }
    }

    public class AuthResult {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityError> IdentityErrors { get; set; } = Enumerable.Empty<IdentityError>();
    }
}
