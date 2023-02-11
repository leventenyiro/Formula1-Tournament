using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public ResultService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, Result? Result, string? ErrorMessage)> GetResultById(Guid id)
        {
            var result = await _carRacingTournamentDbContext.Results
                .Where(e => e.Id == id)
                .Include(x => x.Driver)
                .Include(x => x.Team)
                .Select(x => new Result
                {
                    Id = x.Id,
                    Position = x.Position,
                    Points = x.Points,
                    Driver = new Driver
                    {
                        Id = x.Driver.Id,
                        Name = x.Driver.Name,
                        RealName = x.Driver.RealName,

                    },
                    Team = new Team
                    {
                        Id = x.Team.Id,
                        Name = x.Team.Name,
                        Color = x.Team.Color
                    }
                }).FirstOrDefaultAsync();
            if (result == null)
                return (false, null, _configuration["ErrorMessages:ResultNotFound"]);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto)
        {
            var resultObj = await _carRacingTournamentDbContext.Results.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (resultObj == null)
                return (false, _configuration["ErrorMessages:ResultNotFound"]);

            Race? raceObj = await _carRacingTournamentDbContext.Races.Where(e => e.Id == resultObj.RaceId).FirstOrDefaultAsync();
            Team? teamObj = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == resultDto.TeamId).FirstOrDefaultAsync();
            if (raceObj?.SeasonId != teamObj?.SeasonId)
                return (false, _configuration["ErrorMessages:RaceTeamNotSameSeason"]);

            Driver? driverObj = await _carRacingTournamentDbContext.Drivers.Where(e => e.Id == resultDto.DriverId).FirstOrDefaultAsync();
            if (raceObj?.SeasonId != driverObj?.SeasonId)
                return (false, _configuration["ErrorMessages:RaceDriverNotSameSeason"]);

            if (resultDto.Position <= 0)
                return (false, _configuration["ErrorMessages:ResultPosition"]);

            if (resultDto.Points < 0)
                return (false, _configuration["ErrorMessages:ResultPoints"]);

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
                return (false, _configuration["ErrorMessages:ResultNotFound"]);
            
            _carRacingTournamentDbContext.Results.Remove(result);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }
    }
}
