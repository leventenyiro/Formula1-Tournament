using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DriverService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeason(Season season)
        {
            var drivers = await _carRacingTournamentDbContext.Drivers
                .Where(x => x.SeasonId == season.Id)
                .Include(x => x.ActualTeam)
                .Include(x => x.Results!).ThenInclude(x => x.Race)
                .Include(x => x.Results!).ThenInclude(x => x.Team)
                .Select(x => new Driver
                {
                    Id = x.Id,
                    Name = x.Name,
                    RealName = x.RealName,
                    Number = x.Number,
                    ActualTeam = x.ActualTeam != null ? new Team
                    {
                        Id = x.ActualTeam.Id,
                        Name = x.ActualTeam.Name,
                        Color = x.ActualTeam.Color
                    } : null,
                    Results = x.Results!.Select(x => new Result
                    {
                        Id = x.Id,
                        Position = x.Position,
                        Point = x.Point,
                        Race = new Race
                        {
                            Id = x.Race.Id,
                            Name = x.Race.Name,
                            DateTime = x.Race.DateTime
                        },
                        Team = new Team
                        {
                            Id = x.Team.Id,
                            Name = x.Team.Name,
                            Color = x.Team.Color
                        }
                    }).ToList()
                }).ToListAsync();

            return (true, drivers, null);
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
                        Point = x.Point,
                        Position = x.Position,
                        Team = x.Team
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (driver == null)
                return (false, null, _configuration["ErrorMessages:DriverNotFound"]);
            
            return (true, driver, null);
        }
        public async Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Season season, DriverDto driverDto, Team team)
        {
            driverDto.Name = driverDto.Name.Trim();
            if (string.IsNullOrEmpty(driverDto.Name))
                return (false, _configuration["ErrorMessages:DriverName"]);

            if (driverDto.Number <= 0 || driverDto.Number >= 100)
                return (false, _configuration["ErrorMessages:DriverNumber"]);

            if (driverDto.ActualTeamId != null && season.Id != team.SeasonId)
                return (false, _configuration["ErrorMessages:DriverTeamNotSameSeason"]);

            if (await _carRacingTournamentDbContext.Drivers.Where(x => x.Name == driverDto.Name && x.SeasonId == season.Id).CountAsync() != 0)
                return (false, _configuration["ErrorMessages:DriverNameExists"]);
            
            if (await _carRacingTournamentDbContext.Drivers.Where(x => x.Number == driverDto.Number && x.SeasonId == season.Id).CountAsync() != 0)
                return (false, _configuration["ErrorMessages:DriverNumberExists"]);

            driverDto.RealName = driverDto.RealName?.Trim();
            var driver = _mapper.Map<Driver>(driverDto);
            driver.Id = Guid.NewGuid();
            driver.SeasonId = season.Id;
            await _carRacingTournamentDbContext.Drivers.AddAsync(driver);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriver(Driver driver, DriverDto driverDto, Team team)
        {
            driverDto.Name = driverDto.Name.Trim();
            if (string.IsNullOrEmpty(driverDto.Name))
                return (false, _configuration["ErrorMessages:DriverName"]);

            if (driverDto.Number <= 0 || driverDto.Number >= 100)
                return (false, _configuration["ErrorMessages:DriverNumber"]);

            if (team != null && driver.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:DriverTeamNotSameSeason"]);

            if (driver.Name != driverDto.Name && 
                await _carRacingTournamentDbContext.Drivers.Where(x => x.Name == driverDto.Name && x.SeasonId == driver.SeasonId).CountAsync() != 0)
                return (false, _configuration["ErrorMessages:DriverNameExists"]);
            
            if (driver.Number != driverDto.Number && 
                await _carRacingTournamentDbContext.Drivers.Where(x => x.Number == driverDto.Number && x.SeasonId == driver.SeasonId).CountAsync() != 0)
                return (false, _configuration["ErrorMessages:DriverNumberExists"]);

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
                return (false, _configuration["ErrorMessages:DriverTeamNotSameSeason"]);

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
