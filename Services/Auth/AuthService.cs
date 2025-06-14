﻿using Cinema_Management_System.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Cinema_Management_System.Models.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Cinema_Management_System.Services.Interfaces;

namespace Cinema_Management_System.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationSettings _authSettings;
        public AuthService(SignInManager<ApplicationUser> signInManager,
                    UserManager<ApplicationUser> userManager,
                    AuthenticationSettings authSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authSettings = authSettings;
        }

        public async Task<RegisterResult> RegisterUserAsync(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            var errors = result.Errors.Select(e => e.Description);
            if (result.Succeeded)
            {

                await _userManager.AddToRoleAsync(user, "User");
            }
            return new RegisterResult
            {
                Succeeded = result.Succeeded,
                Errors = errors
            };

        }
        public async Task<IdentityResult> RegisterEmployeeAsync(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Employee");
                if (!roleResult.Succeeded)
                    return IdentityResult.Failed(roleResult.Errors.ToArray());
            }

            return result;
        }
        public async Task<string?> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(_authSettings.JwtExpireDays);

            var token = new JwtSecurityToken(
                issuer: _authSettings.JwtIssuer,
                audience: _authSettings.JwtIssuer,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    

    public class AuthResult {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityError> IdentityErrors { get; set; } = Enumerable.Empty<IdentityError>();
    }
}
