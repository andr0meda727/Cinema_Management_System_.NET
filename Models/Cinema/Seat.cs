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
        public int ScreeningRoomId { get; set; }

        [Required]
        [StringLength(2)]
        public required string Row { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Seat in Row")]
        public int SeatInRow { get; set; }

        [Required]
        public SeatTypes SeatType { get; set; }

        [Required]
        public bool SeatStatus { get; set; } // 0 - free, 1 - occupied

        [ForeignKey("ScreeningRoomId")]
        public required ScreeningRoom ScreeningRoom { get; set; }

        public Ticket? Ticket { get; set; }
    }

}
