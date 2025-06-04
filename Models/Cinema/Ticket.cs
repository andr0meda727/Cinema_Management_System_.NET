using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Management_System.Models.Cinema
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ScreeningId { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        // FinalPrice = BasePrice (different screenings have different base prices) * multiplier (based on the seattype (standard, double, vip)
        public decimal FinalPrice { get; set; } 
        [ForeignKey("ScreeningId")]
        public Screening Screening { get; set; }

        [ForeignKey("SeatId")]
        public Seat Seat { get; set; }
    }

