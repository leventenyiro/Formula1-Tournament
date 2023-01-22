using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IResult
    {
        Task<(bool IsSuccess, Result? Result, string? ErrorMessage)> GetResultById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteResult(Guid id);
    }
}
