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

        public DriverService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
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
                        Type = x.Type,
                        Position = x.Position,
                        Point = x.Point,
                        Team = x.Team
                    }).ToList(),
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

            if (await _carRacingTournamentDbContext.Drivers.CountAsync(
                x => x.Name == driverDto.Name && x.SeasonId == season.Id) != 0)
                return (false, _configuration["ErrorMessages:DriverNameExists"]);

            if (await _carRacingTournamentDbContext.Drivers.CountAsync(
                x => x.Number == driverDto.Number && x.SeasonId == season.Id) != 0)
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
                await _carRacingTournamentDbContext.Drivers.CountAsync(
                    x => x.Name == driverDto.Name && x.SeasonId == driver.SeasonId) != 0)
                return (false, _configuration["ErrorMessages:DriverNameExists"]);

            if (driver.Number != driverDto.Number &&
                await _carRacingTournamentDbContext.Drivers.CountAsync(
                    x => x.Number == driverDto.Number && x.SeasonId == driver.SeasonId) != 0)
                return (false, _configuration["ErrorMessages:DriverNumberExists"]);

            driver.Name = driverDto.Name;
            driver.RealName = driverDto.RealName?.Trim();
            driver.Number = driverDto.Number;
            driver.ActualTeamId = driverDto.ActualTeamId;
            _carRacingTournamentDbContext.Entry(driver).State = EntityState.Modified;
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteDriver(Driver driver)
        {
            _carRacingTournamentDbContext.Drivers.Remove(driver);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> SetActualTeamNullByTeamId(Guid teamId)
        {
            var drivers = await _carRacingTournamentDbContext.Drivers
                .AsNoTracking()
                .Where(x => x.ActualTeamId == teamId)
                .ToListAsync();
            foreach (var driver in drivers)
            {
                driver.ActualTeam = null;
                driver.ActualTeamId = null;
            }
            _carRacingTournamentDbContext.Drivers.UpdateRange(drivers);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool IsSuccess, Statistics? DriverStatistics, string? ErrorMessage)> GetStatistics(string name)
        {
            var driverObj = await _carRacingTournamentDbContext.Drivers.Where(x => x.Name == name).Select(x => x.Name).FirstOrDefaultAsync();
            var driver = new List<Driver>();
            if (!bool.Parse(_configuration["Development"])) {
                driver = await _carRacingTournamentDbContext.Drivers.Where(x => EF.Functions.Collate(x.Name, "SQL_Latin1_General_CP1_CS_AS") == name)
                    .Include(x => x.Results!).ThenInclude(x => x.Team)
                    .Include(x => x.Season)
                    .Include(x => x.ActualTeam)
                    .ToListAsync();
            } else {
                driver = await _carRacingTournamentDbContext.Drivers.Where(x => x.Name == name)
                    .Include(x => x.Results!).ThenInclude(x => x.Team)
                    .Include(x => x.Season)
                    .Include(x => x.ActualTeam)
                    .ToListAsync();
            }

            if (driver.Count == 0)
                return (false, null, _configuration["ErrorMessages:DriverNotFound"]);

            int numberOfChampion = driver.Count(d => _carRacingTournamentDbContext
                .Seasons
                .Where(x => x.Id == d.Season.Id && d.Season.IsArchived == true)
                .Include(x => x.Drivers)!.ThenInclude(x => x.Results)
                .FirstOrDefaultAsync()?.Result?.Drivers?
                .Select(x => new {
                    Id = x.Id,
                    SumPoint = x.Results?.Sum(x => x.Point)
                })
                .OrderByDescending(x => x.SumPoint)
                .FirstOrDefault()?.Id == d.Id
            );

            List<SeasonStatistics> seasonStatistics = driver
                .Select(x => new SeasonStatistics {
                    Name = x.Season.Name,
                    TeamName = x.ActualTeam?.Name,
                    TeamColor = x.ActualTeam?.Color ?? "#000000",
                    CreatedAt = x.Season.CreatedAt,
                    Position = getDriverPositionInSeason(x.Season.Id, x)
                })
                .OrderBy(x => x.CreatedAt)
                .ToList();

            List<PositionStatistics> positionStatistics = driver
                .SelectMany(x => x.Results!)
                .GroupBy(x => x.Position == null ? x.Type.ToString() : x.Position.Value.ToString())
                .Select(x => new PositionStatistics {
                    Position = x.Key,
                    Number = x.Count()
                })
                .OrderBy(x => x.Position, new CustomPositionComparer()!)
                .ToList();

            var driverStat = new Statistics {
                Name = driverObj,
                NumberOfRace = driver.SelectMany(x => x.Results!).Count(),
                NumberOfWin = driver.SelectMany(x => x.Results!).Where(x => x.Position == 1).Count(),
                NumberOfPodium = driver.SelectMany(x => x.Results!).Where(x => x.Position >= 1 && x.Position <= 3).Count(),
                NumberOfChampion = numberOfChampion,
                SumPoint = driver.SelectMany(x => x.Results!).Sum(x => x.Point),
                SeasonStatistics = seasonStatistics,
                PositionStatistics = positionStatistics
            };

            return (true, driverStat, null);
        }

        private int getDriverPositionInSeason(Guid seasonId, Driver driver) {
            var season = _carRacingTournamentDbContext
                .Seasons
                .Where(x => x.Id == seasonId)
                .Include(x => x.Drivers)!.ThenInclude(x => x.Results)
                .FirstOrDefaultAsync()
                .Result;

            var sumPointsDrivers = season!.Drivers!
                .Select((x, i) => new {
                    DriverId = x.Id,
                    SumPoint = x.Results!.Sum(x => x.Point)
                })
                .OrderByDescending(x => x.SumPoint);

            return sumPointsDrivers
                .ToList()
                .IndexOf(
                    sumPointsDrivers
                    .Where(x => x.DriverId == driver.Id)
                    .FirstOrDefault()!
                ) + 1;
        }
    }
}
public class CustomPositionComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        int xInt, yInt;
        if (int.TryParse(x, out xInt) && int.TryParse(y, out yInt))
        {
            return xInt.CompareTo(yInt);
        }
        else
        {
            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
