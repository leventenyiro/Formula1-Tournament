using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;

        private const string RESULT_NOT_FOUND = "Result not found";

        public ResultService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
        }

        public async Task<(bool IsSuccess, Result? Result, string? ErrorMessage)> GetResultById(Guid id)
        {
            var result = await _carRacingTournamentDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (result == null)
                return (false, null, RESULT_NOT_FOUND);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto)
        {
            var resultObj = await _carRacingTournamentDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (resultObj == null)
                return (false, RESULT_NOT_FOUND);
            
            resultObj.Position = resultDto.Position;
            resultObj.Points = resultDto.Points;
            resultObj.DriverId = resultDto.DriverId;
            resultObj.TeamId = resultDto.TeamId;
            _carRacingTournamentDbContext.Results.Update(resultObj);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResult(Guid id)
        {
            var result = await _carRacingTournamentDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (result == null)
                return (false, RESULT_NOT_FOUND);
            
            _carRacingTournamentDbContext.Results.Remove(result);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }
    }
}
