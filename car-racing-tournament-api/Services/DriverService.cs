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

        private const string DRIVER_NOT_FOUND = "Driver not found";

        public DriverService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
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
                })
                .FirstOrDefaultAsync();
            if (driver == null)
                return (false, null, DRIVER_NOT_FOUND);
            
            return (true, driver, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriver(Guid id, DriverDto driverDto)
        {
            var driverObj = await _carRacingTournamentDbContext.Drivers.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (driverObj == null)
                return (false, DRIVER_NOT_FOUND);

            driverObj.Name = driverDto.Name;
            driverObj.RealName = driverDto.RealName;
            driverObj.Number = driverDto.Number;
            driverObj.ActualTeamId = driverDto.ActualTeamId;
            _carRacingTournamentDbContext.Drivers.Update(driverObj);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriverTeam(Guid id, Guid teamId)
        {
            var driverObj = await _carRacingTournamentDbContext.Drivers.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (driverObj == null)
                return (false, DRIVER_NOT_FOUND);
            
            driverObj.ActualTeamId = teamId;
            _carRacingTournamentDbContext.Drivers.Update(driverObj);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteDriver(Guid id)
        {
            var driver = await _carRacingTournamentDbContext.Drivers.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (driver == null)
                return (false, DRIVER_NOT_FOUND);
            
            _carRacingTournamentDbContext.Drivers.Remove(driver);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }
    }
}
