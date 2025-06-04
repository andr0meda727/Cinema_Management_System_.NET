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
        public string Name { get; set; }

        [Required]
        [Display(Name = "Screen Format")]
        public ScreenFormats Format { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Number of Seats")]
        public int NumberOfSeats { get; set; }
    }
}
