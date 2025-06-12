using Cinema_Management_System.Models.Cinema;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Management_System.DTOs.Employee
{
    public class CreateScreeningRoomDTO
    {
        [Required(ErrorMessage = "Wymagana jest nazwa sali.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public ScreenFormats Format { get; set; }

        [Required(ErrorMessage = "Ilość rzędów musi się mieścić pomiędzy 3 - 20.")]
        [Range(3, 10, ErrorMessage = "Ilość rzędów musi się mieścić pomiędzy 3 - 10.")]
        public int Rows { get; set; } = 3;

        [Required(ErrorMessage = "Ilość siedzeń w rzędzie musi się mieścić pomiędzy 5-20")]
        [Range(5, 20, ErrorMessage = "Ilość siedzeń w rzędzie musi się mieścić pomiędzy 5-20")]
        public int SeatsPerRow { get; set; } = 5;
    }
}

