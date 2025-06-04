using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.Models.Users
{
    public class User
    {
        [Key]
        public int Id { get; set; }
       
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
