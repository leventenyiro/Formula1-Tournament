using formula1_tournament_api.DTO;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface ISeason
    {
        Task<(bool IsSuccess, List<SeasonDto> Seasons, string ErrorMessage)> GetAllSeasons();
        Task<(bool IsSuccess, SeasonDto Season, string ErrorMessage)> GetSeasonById(Guid id);
        Task<(bool IsSuccess, Guid SeasonId, string ErrorMessage)> AddSeason(string name, Guid userId);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateSeason(Guid id, string name);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteSeason(Guid id);
        Task<(bool IsSuccess, List<SeasonDto> Seasons, string ErrorMessage)> GetAllSeasonsByUserSeasonList(List<Guid> userSeasons);
    }
}
