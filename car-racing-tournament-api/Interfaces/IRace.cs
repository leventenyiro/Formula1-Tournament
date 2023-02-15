using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IRace
    {
        Task<(bool IsSuccess, List<Race>? Races, string? ErrorMessage)> GetRacesBySeason(Season season);
        Task<(bool IsSuccess, Race? Race, string? ErrorMessage)> GetRaceById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Season season, RaceDto raceDto);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Race race, RaceDto raceDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Race race);
    }
}
