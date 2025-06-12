using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Employee
{
    public class AddScreeningDTO
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        public int ScreeningRoomId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required]
        [Range(0.01, 1000.00)]
        public decimal BasePrice { get; set; }
    }
}
