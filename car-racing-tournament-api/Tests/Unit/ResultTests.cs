using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
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

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            Season season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "First Season"
            };

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                Season = season
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

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _resultService = new ResultService(_context, configuration);
        }

        [Test]
        public async Task GetResultByIdSuccess()
        {
            var result = await _resultService!.GetResultById(_result!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Result resultOutput = result.Result!;
            Assert.AreEqual(resultOutput.Id, _context!.Results.First().Id);
            Assert.AreEqual(resultOutput.Points, _context!.Results.First().Points);
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
        public async Task UpdateResultSuccess()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = 18,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            var findResult = _context!.Results.FirstAsync().Result;
            Assert.AreEqual(findResult.Points, resultDto.Points);
            Assert.AreEqual(findResult.Position, resultDto.Position);
            Assert.AreEqual(findResult.DriverId, resultDto.DriverId);
            Assert.AreEqual(findResult.TeamId, resultDto.TeamId);
        }

        // update - result already exists

        [Test]
        public async Task UpdateResultNotPositivePosition()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = 18,
                Position = -1,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);

            resultDto.Position = 0;
            result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task UpdateResultMinusPoints()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = -2,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, _context.Races.First(), driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task UpdateDriverWithAnotherSeason()
        {
            var anotherSeasonId = Guid.NewGuid();
            Race race = new Race
            {
                Id = Guid.NewGuid(),
                Name = "Another race",
                SeasonId = Guid.NewGuid()
            };

            Driver driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "AnotherDriver",
                Number = 3,
                SeasonId = anotherSeasonId
            };

            var resultDto = new ResultDto
            {
                Points = 5,
                Position = 5,
                DriverId = Guid.NewGuid(),
                TeamId = _context!.Teams.FirstOrDefaultAsync().Result!.Id,
            };

            var result = await _resultService!.UpdateResult(_result!, resultDto, race, driver, _context.Teams.FirstOrDefaultAsync().Result!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "AnotherTeam",
                Color = "123123",
                SeasonId = anotherSeasonId
            };

            resultDto.DriverId = _context.Drivers.FirstOrDefaultAsync().Result!.Id;
            resultDto.TeamId = team.Id;

            result = await _resultService!.UpdateResult(_result!, resultDto, race, _context.Drivers.FirstOrDefaultAsync().Result!, team);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
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
