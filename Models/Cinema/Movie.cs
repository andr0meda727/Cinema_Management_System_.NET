using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.Models.Cinema
{
    public enum AgeCategory
    {
        [Display(Name = "For everyone")]
        General,

        [Display(Name = "7+")]
        SevenPlus,

        [Display(Name = "12+")]
        TwelvePlus,

        [Display(Name = "16+")]
        SixteenPlus,

        [Display(Name = "Adults (18+)")]
        EighteenPlus,
    }

    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 1000)]
        public int MovieLength { get; set; }

        [Required]
        [StringLength(100)]
        public AgeCategory AgeCategory { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
