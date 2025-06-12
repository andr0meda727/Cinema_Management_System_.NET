using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.DTOs.Employee
{
    public class EditScreeningRoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ScreenFormats Format { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
    }
}
