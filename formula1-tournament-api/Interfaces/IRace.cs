using formula1_tournament_api.DTO;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IRace
    {
        Task<(bool IsSuccess, List<Race> Races, string ErrorMessage)> GetAllRacesBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, Race Race, string ErrorMessage)> GetRaceById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddRace(RaceDto raceDto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateRace(Guid id, RaceDto raceDto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteRace(Guid id);
    }
}
