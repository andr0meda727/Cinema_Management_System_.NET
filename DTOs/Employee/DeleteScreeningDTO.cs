namespace Cinema_Management_System.DTOs.Employee
{
    public class DeleteScreeningDTO
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; }
        public string RoomName { get; set; }
        public DateTime DateStartTime { get; set; }
    }
}
