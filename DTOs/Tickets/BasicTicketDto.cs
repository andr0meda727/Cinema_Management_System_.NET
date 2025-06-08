namespace Cinema_Management_System.DTOs.Tickets
{
    public class BasicTicketDTO
    {
        public int Id { get; set; }
        public string? MovieTitle { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime DateStartTime { get; set; }
    }
}
