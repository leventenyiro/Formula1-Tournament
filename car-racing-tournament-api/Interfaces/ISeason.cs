using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface ISeason
    {
        Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasons();
        Task<(bool IsSuccess, Season? Season, string? ErrorMessage)> GetSeasonById(Guid id);
        Task<(bool IsSuccess, SeasonOutputDto? Season, string? ErrorMessage)> GetSeasonByIdWithDetails(Guid id);
        Task<(bool IsSuccess, Guid SeasonId, string? ErrorMessage)> AddSeason(SeasonCreateDto seasonDto, Guid userId);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateSeason(Season season, SeasonUpdateDto seasonDto);
        Task<(bool IsSuccess, string? ErrorMessage)> ArchiveSeason(Season season);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteSeason(Season season);

        Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasonsByUserId(Guid userId);

        Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeasonId(Season season);
        Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Season season, DriverDto driverDto, Team team);

        Task<(bool IsSuccess, List<Team>? Teams, string? ErrorMessage)> GetTeamsBySeasonId(Season season);
        Task<(bool IsSuccess, string? ErrorMessage)> AddTeam(Season season, TeamDto team);

        Task<(bool IsSuccess, List<Race>? Races, string? ErrorMessage)> GetRacesBySeasonId(Season season);
        Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Season season, RaceDto raceDto);
    }
}
