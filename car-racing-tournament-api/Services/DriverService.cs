using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class DriverService : IDriver
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public DriverService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, Driver? Driver, string? ErrorMessage)> GetDriverById(Guid id)
        {
            var driver = await _carRacingTournamentDbContext.Drivers.Where(e => e.Id == id)
                .Include(e => e.Results!)
                .ThenInclude(x => x.Team)
                .Select(x => new Driver
                {
                    Id = x.Id,
                    Name = x.Name,
                    RealName = x.RealName,
                    Number = x.Number,
                    ActualTeamId = x.ActualTeamId,
                    SeasonId = x.SeasonId,
                    Results = x.Results!.Select(x => new Result
                    {
                        Id = x.Id,
                        Points = x.Points,
                        Position = x.Position,
                        Team = x.Team
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (driver == null)
                return (false, null, _configuration["ErrorMessages:DriverNotFound"]);
            
            return (true, driver, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriver(Driver driver, DriverDto driverDto, Team team)
        {
            if (driverDto == null)
                return (false, _configuration["ErrorMessages:MissingDriver"]);

            driverDto.Name = driverDto.Name.Trim();
            if (string.IsNullOrEmpty(driverDto.Name))
                return (false, _configuration["ErrorMessages:DriverName"]);

            if (driverDto.Number <= 0 || driverDto.Number >= 100)
                return (false, _configuration["ErrorMessages:DriverNumber"]);

            if (team != null && driver.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:DriverTeamNotTheSameSeason"]);

            driver.Name = driverDto.Name;
            driver.RealName = driverDto.RealName?.Trim();
            driver.Number = driverDto.Number;
            driver.ActualTeamId = driverDto.ActualTeamId;
            _carRacingTournamentDbContext.Drivers.Update(driver);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriverTeam(Driver driver, Team team)
        {
            if (team != null && driver.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:DriverTeamNotTheSameSeason"]);

            driver.ActualTeam = team != null ? team : null;
            _carRacingTournamentDbContext.Drivers.Update(driver);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteDriver(Driver driver)
        {
            _carRacingTournamentDbContext.Drivers.Remove(driver);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }
    }
}
