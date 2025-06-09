namespace Cinema_Management_System.DTOs.Screening
{
    public class BasicScreeningDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime DateStartTime { get; set; }
        public DateTime DateEndTime { get; set; }
        public string? ShortDescription { get; set; }
        public required string MoviePosterUrl { get; set; }
    }
}
