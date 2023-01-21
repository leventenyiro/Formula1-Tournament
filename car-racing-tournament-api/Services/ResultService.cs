using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly FormulaDbContext _formulaDbContext;

        public ResultService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, Result Result, string ErrorMessage)> GetResultById(Guid id)
        {
            var result = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (result != null)
            {
                return (true, result, null);
            }
            return (false, null, "Result not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto)
        {
            var resultObj = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (resultObj != null)
            {
                resultObj.Position = resultDto.Position;
                resultObj.Points = resultDto.Points;
                resultObj.DriverId = resultDto.DriverId;
                resultObj.TeamId = resultDto.TeamId;
                _formulaDbContext.Results.Update(resultObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Result not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteResult(Guid id)
        {
            var result = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (result != null)
            {
                _formulaDbContext.Results.Remove(result);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Result not found");
        }
    }
}
