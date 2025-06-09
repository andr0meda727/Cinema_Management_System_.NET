using System;
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
        public ScreenFormats Format { get; set; }

        [Required]
        [Range(3, 100)] // min 3 rows
        [Display(Name = "Number of Rows")]
        public int Rows { get; set; }

        [Required]
        [Range(5, 100)] // min 5 seats per row
        [Display(Name = "Seats Per Row")]
        public int SeatsPerRow { get; set; }
        public int NumberOfSeats { get; set; }

        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
