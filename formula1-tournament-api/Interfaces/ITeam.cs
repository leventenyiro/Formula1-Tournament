using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface ITeam
    {
        Task<(bool IsSuccess, List<Team> Teams, string ErrorMessage)> GetAllTeamsBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, Team Team, string ErrorMessage)> GetTeamById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddTeam(Team team);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateTeam(Guid id, Team team);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteTeam(Guid id);
    }
}
