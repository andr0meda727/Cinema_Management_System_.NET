using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Employee
{
    public class AddMovieDTO
    {
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        [StringLength(5000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Długość filmu jest wymagana.")]
        [Range(1, 1000)]
        [Display(Name = "Movie Length (minutes)")]
        public int MovieLength { get; set; }

        [Required(ErrorMessage = "Kategoria wiekowa jest wymagana.")]
        [Display(Name = "Age Category")]
        public AgeCategory AgeCategory { get; set; }

        [Required(ErrorMessage = "Plakat jest wymagany.")]
        [Display(Name = "Movie Poster")]
        public IFormFile? PosterFile { get; set; }
    }
}
