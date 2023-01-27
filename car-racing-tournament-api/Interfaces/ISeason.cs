using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface ISeason
    {
        Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasons();
        Task<(bool IsSuccess, SeasonOutputDto? Season, string? ErrorMessage)> GetSeasonById(Guid id);
        Task<(bool IsSuccess, Guid SeasonId, string? ErrorMessage)> AddSeason(SeasonDto seasonDto, Guid userId);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateSeason(Guid id, SeasonDto seasonDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteSeason(Guid id);

        Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasonsByUserSeasonList(List<Guid> userSeasons);

        Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Guid seasonId, DriverDto driverDto);

        Task<(bool IsSuccess, List<Team>? Teams, string? ErrorMessage)> GetTeamsBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, string? ErrorMessage)> AddTeam(Guid seasonId, TeamDto team);

        Task<(bool IsSuccess, List<Race>? Races, string? ErrorMessage)> GetRacesBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Guid seasonId, RaceDto raceDto);
    }
}
