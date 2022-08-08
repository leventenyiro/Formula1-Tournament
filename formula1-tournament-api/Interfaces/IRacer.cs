using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IRacer
    {
        Task<(bool IsSuccess, List<Racer> Racer, string ErrorMessage)> GetAllRacers();
        Task<(bool IsSuccess, Racer Racer, string ErrorMessage)> GetRacerById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddRacer(Racer racer);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateRacer(Guid id, Racer racer);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteRacer(Guid id);
    }
}
