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
        public string Row { get; set; }

        [Required]
        [Range(1, 1000)]
        public int SeatNumber { get; set; }

        [Required]
        public SeatTypes SeatType { get; set; }

        [Required]
        public bool SeatStatus { get; set; } // 0 - free, 1 - occupied

        [ForeignKey("ScreeningRoomId")]
        public ScreeningRoom ScreeningRoom { get; set; }

        public Ticket Ticket { get; set; }
    }

}
