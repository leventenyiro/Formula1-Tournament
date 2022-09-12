using formula1_tournament_api.DTO;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface ISeason
    {
        Task<(bool IsSuccess, List<Season> Seasons, string ErrorMessage)> GetAllSeasons();
        Task<(bool IsSuccess, Season Season, string ErrorMessage)> GetSeasonById(Guid id);
        Task<(bool IsSuccess, Guid SeasonId, string ErrorMessage)> AddSeason(SeasonDto season, Guid userId);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateSeason(Guid id, SeasonDto season);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteSeason(Guid id);
        Task<(bool IsSuccess, List<Season> Seasons, string ErrorMessage)> GetAllSeasonsByList(List<UserSeason> userSeasons);
    }
}
