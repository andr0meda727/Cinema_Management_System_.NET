using Cinema_Management_System.DTOs.Screening;

namespace Cinema_Management_System.Models
{
    public class ScreeningViewModel
    {
        public DateTime SelectedDate { get; set; }
        public List<BasicScreeningDTO> Screenings { get; set; } = new();
    }
}
