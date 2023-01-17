using api.DTO;
using api.Models;

namespace api.Interfaces
{
    public interface ITeam
    {
        Task<(bool IsSuccess, List<Team> Teams, string ErrorMessage)> GetAllTeamsBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, Team Team, string ErrorMessage)> GetTeamById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddTeam(TeamDto team);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateTeam(Guid id, TeamDto team);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteTeam(Guid id);
    }
}
