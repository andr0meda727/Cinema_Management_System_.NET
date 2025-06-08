using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Employee
{
    public class CreateScreeningRoomDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public ScreenFormats Format { get; set; }

        [Required]
        [Range(3, 100)]
        public int Rows { get; set; } = 3;

        [Required]
        [Range(5, 100)]
        public int SeatsPerRow { get; set; } = 5;
    }
}

