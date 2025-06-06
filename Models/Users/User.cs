using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cinema_Management_System.Models.Users
{
    public class User : IdentityUser
    {
        [Required]
        public string Nick { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual Role? Role { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
