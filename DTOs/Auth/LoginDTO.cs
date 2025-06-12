using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Auth
{
    public class LoginDTO
    {
        [Required]
        public required string UserName { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
