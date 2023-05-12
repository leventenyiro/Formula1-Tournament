using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface ITeam
    {
        Task<(bool IsSuccess, Team? Team, string? ErrorMessage)> GetTeamById(Guid id);
        Task<(bool IsSuccess, Guid? Id, string? ErrorMessage)> AddTeam(Season season, TeamDto team);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateTeam(Team team, TeamDto teamDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteTeam(Team team);
    }
}
