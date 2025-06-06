using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Nick { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
