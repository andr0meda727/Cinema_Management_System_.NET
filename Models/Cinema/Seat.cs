using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Management_System.Models.Cinema
{
    public enum SeatTypes
    {
        Standard,
        Double,
        Vip
    }

    public enum SeatStatus
    {
        Free,
        Occupied
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
        public SeatStatus SeatStatus { get; set; }

        [ForeignKey("ScreeningRoomId")]
        public ScreeningRoom ScreeningRoom { get; set; }
    }

}
