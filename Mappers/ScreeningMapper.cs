using Cinema_Management_System.DTOs.Screening;
using Riok.Mapperly.Abstractions;

namespace Cinema_Management_System.Mappers;

[Mapper]
public partial class ScreeningMapper
{
    [MapProperty(nameof(Screening.Movie.Title), nameof(BasicScreeningDTO.Title))]
    [MapProperty(nameof(Screening.Movie.ImagePath), nameof(BasicScreeningDTO.MoviePosterUrl))]
    [MapperIgnoreSource(nameof(Screening.MovieId))]
    [MapperIgnoreSource(nameof(Screening.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Screening.ScreeningRoom))]
    [MapperIgnoreSource(nameof(Screening.BasePrice))]
    [MapperIgnoreSource(nameof(Screening.Tickets))]
    [MapperIgnoreTarget(nameof(BasicScreeningDTO.ShortDescription))]
    public partial BasicScreeningDTO ScreeningToScreeningBasicDTO(Screening screening);
    public BasicScreeningDTO ScreeningToBasicScreeningDTOWithShortDesc(Screening screening)
    {
        var dto = ScreeningToScreeningBasicDTO(screening);
        dto.ShortDescription = TruncateDescription(screening.Movie?.Description ?? string.Empty);
        return dto;
    }

    [MapProperty(nameof(Screening.Movie.Title), nameof(BasicScreeningDTO.Title))]
    [MapProperty(nameof(Screening.Movie.ImagePath), nameof(BasicScreeningDTO.MoviePosterUrl))]
    [MapProperty(nameof(Screening.Movie.Description), nameof(DetailedScreeningDTO.MovieDescription))]
    [MapProperty(nameof(Screening.Movie.AgeCategory), nameof(DetailedScreeningDTO.AgeCategory))]
    [MapProperty(nameof(Screening.ScreeningRoom.Name), nameof(DetailedScreeningDTO.ScreeningRoomName))]
    [MapProperty(nameof(Screening.ScreeningRoom.Format), nameof(DetailedScreeningDTO.ScreeningRoomFormat))]
    [MapperIgnoreSource(nameof(Screening.ScreeningRoomId))]
    [MapperIgnoreSource(nameof(Screening.Tickets))]
    [MapperIgnoreSource(nameof(Screening.MovieId))]
    [MapperIgnoreTarget(nameof(DetailedScreeningDTO.ShortDescription))]
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
