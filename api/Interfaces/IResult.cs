using api.DTO;
using api.Models;

namespace api.Interfaces
{
    public interface IResult
    {
        Task<(bool IsSuccess, List<Result> Results, string ErrorMessage)> GetAllResultsByRaceId(Guid raceId);
        Task<(bool IsSuccess, Result Result, string ErrorMessage)> GetResultById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddResult(ResultDto resultDto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteResult(Guid id);
    }
}
