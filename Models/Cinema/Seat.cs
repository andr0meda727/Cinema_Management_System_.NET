using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Management_System.Models.Cinema
{
    public enum SeatTypes
    {
        STANDARD,
        DOUBLE,
        VIP
    }

    public class Seat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int ScreeningRoomId { get; set; }

        [Required]
        [StringLength(2)]
        public required string Row { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Seat in Row")]
        public required int SeatInRow { get; set; }

        [Required]
        public required SeatTypes SeatType { get; set; }

        [ForeignKey("ScreeningRoomId")]
        public ScreeningRoom? ScreeningRoom { get; set; }
    }
}
