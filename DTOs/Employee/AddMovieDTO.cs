using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Employee
{
    public class AddMovieDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Movie Length (minutes)")]
        public int MovieLength { get; set; }

        [Required]
        [Display(Name = "Age Category")]
        public AgeCategory AgeCategory { get; set; }

        [Display(Name = "Movie Poster")]
        public IFormFile? PosterFile { get; set; }
    }
}
