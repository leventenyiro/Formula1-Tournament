using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly CarRacingDbContext _carRacingDbContext;

        private const string RESULT_NOT_FOUND = "Result not found";

        public ResultService(CarRacingDbContext carRacingDbContext)
        {
            _carRacingDbContext = carRacingDbContext;
        }

        public async Task<(bool IsSuccess, Result? Result, string? ErrorMessage)> GetResultById(Guid id)
        {
            var result = await _carRacingDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (result == null)
                return (false, null, RESULT_NOT_FOUND);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto)
        {
            var resultObj = await _carRacingDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (resultObj == null)
                return (false, RESULT_NOT_FOUND);
            
            resultObj.Position = resultDto.Position;
            resultObj.Points = resultDto.Points;
            resultObj.DriverId = resultDto.DriverId;
            resultObj.TeamId = resultDto.TeamId;
            _carRacingDbContext.Results.Update(resultObj);
            _carRacingDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResult(Guid id)
        {
            var result = await _carRacingDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (result == null)
                return (false, RESULT_NOT_FOUND);
            
            _carRacingDbContext.Results.Remove(result);
            _carRacingDbContext.SaveChanges();

            return (true, null);
        }
    }
}
