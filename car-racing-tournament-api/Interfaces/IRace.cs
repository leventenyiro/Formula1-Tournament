using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IRace
    {
        Task<(bool IsSuccess, Race? Race, string? ErrorMessage)> GetRaceById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Guid id, RaceDto raceDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Guid id);

        Task<(bool IsSuccess, List<Result>? Results, string? ErrorMessage)> GetResultsByRaceId(Guid raceId);
        Task<(bool IsSuccess, string? ErrorMessage)> AddResult(Guid raceId, ResultDto resultDto);
    }
}
