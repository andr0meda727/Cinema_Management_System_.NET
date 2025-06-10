namespace Cinema_Management_System.DTOs.Employee
{
    public class BrowseScreeningDTO
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; }
        public string RoomName { get; set; }
        public DateTime DateStartTime { get; set; }
        public DateTime DateEndTime { get; set; }
        public decimal BasePrice { get; set; }
    }
}
