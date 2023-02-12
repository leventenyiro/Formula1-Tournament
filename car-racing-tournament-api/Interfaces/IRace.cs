using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IRace
    {
        Task<(bool IsSuccess, Race? Race, string? ErrorMessage)> GetRaceById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Race race, RaceDto raceDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Race race);

        Task<(bool IsSuccess, List<Result>? Results, string? ErrorMessage)> GetResultsByRaceId(Race race);
        Task<(bool IsSuccess, string? ErrorMessage)> AddResult(Race race, ResultDto resultDto, Driver driver, Team team);
    }
}
