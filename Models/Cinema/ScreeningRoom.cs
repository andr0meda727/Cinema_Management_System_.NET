using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.Models.Cinema
{
    public enum ScreenFormats
    {
        [Display(Name = "2D")]
        TwoD,

        [Display(Name = "3D")]
        ThreeD,

        [Display(Name = "IMAX")]
        Imax
    }

    public class ScreeningRoom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [Display(Name = "Screen Format")]
        public required ScreenFormats Format { get; set; }

        [Required]
        [Range(3, 20)] // min 3 rows
        [Display(Name = "Number of Rows")]
        public required int Rows { get; set; }

        [Required]
        [Range(5, 20)] // min 5 seats per row
        [Display(Name = "Seats Per Row")]
        public required int SeatsPerRow { get; set; }
        public required int NumberOfSeats { get; set; }

        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
