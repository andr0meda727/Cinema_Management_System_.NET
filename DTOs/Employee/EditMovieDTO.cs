using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Employee
{
    public class EditMovieDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        public int MovieLength { get; set; }

        [Required]
        public AgeCategory AgeCategory { get; set; }

        public IFormFile? PosterFile { get; set; }

        public string? ExistingImagePath { get; set; }
    }
}
