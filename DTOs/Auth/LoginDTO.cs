using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Auth
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
