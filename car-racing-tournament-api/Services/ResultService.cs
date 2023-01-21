using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly FormulaDbContext _formulaDbContext;

        private const string RESULT_NOT_FOUND = "Result not found";

        public ResultService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, Result? Result, string? ErrorMessage)> GetResultById(Guid id)
        {
            var result = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (result == null)
                return (false, null, RESULT_NOT_FOUND);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto)
        {
            var resultObj = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (resultObj == null)
                return (false, RESULT_NOT_FOUND);
            
            resultObj.Position = resultDto.Position;
            resultObj.Points = resultDto.Points;
            resultObj.DriverId = resultDto.DriverId;
            resultObj.TeamId = resultDto.TeamId;
            _formulaDbContext.Results.Update(resultObj);
            _formulaDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResult(Guid id)
        {
            var result = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (result == null)
                return (false, RESULT_NOT_FOUND);
            
            _formulaDbContext.Results.Remove(result);
            _formulaDbContext.SaveChanges();

            return (true, null);
        }
    }
}
