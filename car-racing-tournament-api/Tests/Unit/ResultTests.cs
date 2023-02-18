using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class ResultTests
    {
        private CarRacingTournamentDbContext? _context;
        private ResultService? _resultService;
        private Result? _result;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            var seasonId = Guid.NewGuid();

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                SeasonId = seasonId
            };

            Season season = new Season
            {
                Id = seasonId,
                Name = "First Season",
                Drivers = new List<Driver>()
                {
                    new Driver
                    {
                        Id = Guid.NewGuid(),
                        Name = "SecondDriver",
                        Number = 2,
                        ActualTeam = team
                    }
                }
            };

            _result = new Result
            {
                Id = Guid.NewGuid(),
                Driver = new Driver
                {
                    Id = Guid.NewGuid(),
                    Name = "FirstDriver",
                    Number = 1,
                    ActualTeam = team,
                    Season = season
                },
                Race = new Race
                {
                    Id = Guid.NewGuid(),
                    Name = "FirstRace",
                    Season = season
                },
                Team = team
            };

            _context.Results.Add(_result);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _resultService = new ResultService(_context, mapper, _configuration);
        }

        [Test]
        public async Task GetResultsByRaceSuccess()
        {
            var result = await _resultService!.GetResultsByRace(_context!.Races.First());
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Results);
            Assert.AreEqual(result.Results!.Count, 1);
        }

        [Test]
        public async Task GetResultByIdSuccess()
        {
            var result = await _resultService!.GetResultById(_result!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Result resultOutput = result.Result!;
            Assert.AreEqual(resultOutput.Id, _context!.Results.First().Id);
            Assert.AreEqual(resultOutput.Point, _context!.Results.First().Point);
            Assert.AreEqual(resultOutput.Position, _context!.Results.First().Position);
        }

        [Test]
        public async Task GetResultByIdNotFound()
        {
            var result = await _resultService!.GetResultById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async Task AddResultSuccess()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = 18,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.AddResult(_context!.Races.First(), resultDto, driver, driver.ActualTeam!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 2);
        }

        [Test]
        public async Task AddResultExists() {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = 18,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.AddResult(_context!.Races.First(), resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultExists"]);
        }

        [Test]
        public async Task AddResultNotPositivePosition()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = 18,
                Position = -1,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var race = _context.Races.First();

            var result = await _resultService!.AddResult(race, resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultPosition"]);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);

            resultDto.Position = 0;
            result = await _resultService!.AddResult(race, resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultPosition"]);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task AddResultMinusPoint()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = -2,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };
            
            var result = await _resultService!.AddResult(_context.Races.First(), resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultPoint"]);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task AddResultWithAnotherSeason()
        {
            var anotherSeasonId = Guid.NewGuid();

            Driver driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "AnotherDriver",
                Number = 3,
                SeasonId = anotherSeasonId
            };

            var resultDto = new ResultDto
            {
                Point = 5,
                Position = 5,
                DriverId = driver.Id,
                TeamId = _context!.Teams.FirstOrDefaultAsync().Result!.Id,
            };

            var race = _context.Races.First();

            var result = await _resultService!.AddResult(race, resultDto, driver, _context.Teams.FirstOrDefaultAsync().Result!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceDriverNotSameSeason"]);

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "AnotherTeam",
                Color = "123123",
                SeasonId = anotherSeasonId
            };

            resultDto.DriverId = _context.Drivers.FirstOrDefaultAsync().Result!.Id;
            resultDto.TeamId = team.Id;

            result = await _resultService!.AddResult(race, resultDto, _context.Drivers.FirstOrDefaultAsync().Result!, team);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceTeamNotSameSeason"]);
        }

        [Test]
        public async Task UpdateResultSuccess()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = 18,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            var findResult = _context!.Results.FirstAsync().Result;
            Assert.AreEqual(findResult.Point, resultDto.Point);
            Assert.AreEqual(findResult.Position, resultDto.Position);
            Assert.AreEqual(findResult.DriverId, resultDto.DriverId);
            Assert.AreEqual(findResult.TeamId, resultDto.TeamId);
        }

        [Test]
        public async Task UpdateResultExists() {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var race = _context.Races.First();

            var result = new Result
            {
                Id = Guid.NewGuid(),
                Point = 18,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!,
                RaceId = race.Id
            };

            var resultDto = new ResultDto
            {
                Point = 18,
                Position = 2,
                DriverId = _result!.DriverId,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var resultUpdate = await _resultService!.UpdateResult(result, resultDto, race, driver, driver.ActualTeam!);
            Assert.IsFalse(resultUpdate.IsSuccess);
            Assert.AreEqual(resultUpdate.ErrorMessage, _configuration!["ErrorMessages:ResultExists"]);
        }

        [Test]
        public async Task UpdateResultNotPositivePosition()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = 18,
                Position = -1,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultPosition"]);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);

            resultDto.Position = 0;
            result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultPosition"]);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task UpdateResultMinusPoint()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Point = -2,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:ResultPoint"]);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task UpdateDriverWithAnotherSeason()
        {
            var anotherSeasonId = Guid.NewGuid();

            Driver driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "AnotherDriver",
                Number = 3,
                SeasonId = anotherSeasonId
            };

            var resultDto = new ResultDto
            {
                Point = 5,
                Position = 5,
                DriverId = driver.Id,
                TeamId = _context!.Teams.Where(x => x.SeasonId == _context.Seasons.First().Id).First().Id,
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, _context.Teams.FirstOrDefaultAsync().Result!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceDriverNotSameSeason"]);

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "AnotherTeam",
                Color = "123123",
                SeasonId = anotherSeasonId
            };

            resultDto.DriverId = _context!.Drivers.Where(x => x.SeasonId == _context.Seasons.First().Id).First().Id;
            resultDto.TeamId = team.Id;

            result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), _context.Drivers.FirstOrDefaultAsync().Result!, team);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceTeamNotSameSeason"]);
        }

        [Test]
        public async Task DeleteResultSuccess()
        {
            Assert.IsNotEmpty(_context!.Results);

            var result = await _resultService!.DeleteResult(_result!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Results);
        }
    }
}
