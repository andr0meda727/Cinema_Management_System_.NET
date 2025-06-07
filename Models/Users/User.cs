using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cinema_Management_System.Models.Users
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
