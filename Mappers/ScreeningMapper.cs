using Riok.Mapperly.Abstractions;
using Cinema_Management_System.DTOs.Screening;

namespace Cinema_Management_System.Mappers
{
    [Mapper]
    public partial class ScreeningMapper
    {
        [MapProperty(nameof(Screening.Movie.Title), nameof(BasicScreeningDTO.Title))]
        [MapProperty(nameof(Screening.Movie.ImagePath), nameof(BasicScreeningDTO.MoviePosterUrl))]
        public partial BasicScreeningDTO ScreeningToScreeningBasicDTO(Screening screening);

        [MapProperty(nameof(Screening.Movie.Title), nameof(BasicScreeningDTO.Title))]
        [MapProperty(nameof(Screening.Movie.ImagePath), nameof(BasicScreeningDTO.MoviePosterUrl))]
        [MapProperty(nameof(Screening.Movie.Description), nameof(DetailedScreeningDTO.MovieDescription))]
        [MapProperty(nameof(Screening.Movie.AgeCategory), nameof(DetailedScreeningDTO.AgeCategory))]
        [MapProperty(nameof(Screening.ScreeningRoom.Name), nameof(DetailedScreeningDTO.ScreeningRoomName))]
        [MapProperty(nameof(Screening.ScreeningRoom.Format), nameof(DetailedScreeningDTO.ScreeningRoomFormat))]
        public partial DetailedScreeningDTO ScreeningToScreeningDetailedDTO(Screening screening);

    }
}