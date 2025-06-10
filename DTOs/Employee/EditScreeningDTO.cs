namespace Cinema_Management_System.DTOs.Employee
{
    public class EditScreeningDTO
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int ScreeningRoomId { get; set; }

        public DateTime DateStartTime { get; set; }

        public decimal BasePrice { get; set; }
    }
}
