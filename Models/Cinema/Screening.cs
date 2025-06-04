using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Screening
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MovieId { get; set; }

    [Required]
    public int ScreeningRoomId { get; set; }

    [Required]
    public DateTime DateStartTime { get; set; }

    [Required]
    public DateTime DateEndTime { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal BasePrice { get; set; }

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    [ForeignKey("ScreeningRoomId")]
    public ScreeningRoom ScreeningRoom { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
}
