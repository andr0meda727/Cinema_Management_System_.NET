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
        public BasicScreeningDTO ScreeningToBasicScreeningDTOWithShortDesc(Screening screening)
        {
            var dto = ScreeningToScreeningBasicDTO(screening);
            dto.ShortDescription = TruncateDescription(screening.Movie.Description);
            return dto;
        }

        [MapProperty(nameof(Screening.Movie.Title), nameof(BasicScreeningDTO.Title))]
        [MapProperty(nameof(Screening.Movie.ImagePath), nameof(BasicScreeningDTO.MoviePosterUrl))]
        [MapProperty(nameof(Screening.Movie.Description), nameof(DetailedScreeningDTO.MovieDescription))]
        [MapProperty(nameof(Screening.Movie.AgeCategory), nameof(DetailedScreeningDTO.AgeCategory))]
        [MapProperty(nameof(Screening.ScreeningRoom.Name), nameof(DetailedScreeningDTO.ScreeningRoomName))]
        [MapProperty(nameof(Screening.ScreeningRoom.Format), nameof(DetailedScreeningDTO.ScreeningRoomFormat))]
        public partial DetailedScreeningDTO ScreeningToScreeningDetailedDTO(Screening screening);


        private static string TruncateDescription(string description, int maxLength = 100)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            return description.Length <= maxLength
                ? description
                : description[..maxLength] + "...";
        }
    }
}