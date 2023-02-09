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
        private Guid _resultId;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _resultId = Guid.NewGuid();

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

            _context.Results.Add(new Result
            {
                Id = _resultId,
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
                }
            });
            _context.SaveChanges();

            _resultService = new ResultService(_context);
        }

        [Test]
        public async Task GetResultByIdSuccess()
        {

        }

        [Test]
        public async Task GetRaceByIdNotFound()
        {

        }

        // UpdateResult
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

            var result = await _resultService!.UpdateResult(_resultId, resultDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            var findResult = _context!.Results.FirstAsync().Result;
            Assert.AreEqual(findResult.Points, resultDto.Points);
            Assert.AreEqual(findResult.Position, resultDto.Position);
            Assert.AreEqual(findResult.DriverId, resultDto.DriverId);
            Assert.AreEqual(findResult.TeamId, resultDto.TeamId);
        }

        [Test]
        public async Task UpdateResultMissingIds()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = 18,
                Position = 2,
                DriverId = Guid.Empty,
                TeamId = (Guid)driver!.ActualTeamId!
            };

            var result = await _resultService!.UpdateResult(_resultId, resultDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);

            resultDto.DriverId = driver!.Id;
            resultDto.TeamId = Guid.Empty;
            result = await _resultService!.UpdateResult(_resultId, resultDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

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

            var result = await _resultService!.UpdateResult(_resultId, resultDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);

            resultDto.Position = 0;
            result = await _resultService!.UpdateResult(_resultId, resultDto);
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

            var result = await _resultService!.UpdateResult(_resultId, resultDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        /*[Test] ADD RESULT WITH ANOTHER SEASON ID
        public async Task AddDriverWithAnotherSeasonTeam()
        {
            var anotherSeasonId = Guid.NewGuid();
            _context!.Seasons.Add(new Season
            {
                Id = anotherSeasonId,
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                UserSeasons = new List<UserSeason>()
                {
                    new UserSeason
                    {
                        Id = Guid.NewGuid(),
                        UserId = _userId,
                        Permission = UserSeasonPermission.Admin
                    }
                },
                Teams = new List<Team>()
                {
                    new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Team",
                        Color = "FF0000",
                        Drivers = new List<Driver>(),
                        Results = new List<Result>(),
                        SeasonId = anotherSeasonId
                    }
                },
                Drivers = new List<Driver>(),
                Races = new List<Race>()
            });
            _context.SaveChanges();

            var driverDto = new DriverDto
            {
                Name = "AddDriver1",
                RealName = "Add Driver",
                Number = 2,
                ActualTeamId = _context.Seasons
                    .Where(x => x.Id == anotherSeasonId)
                    .FirstOrDefaultAsync().Result!.Teams!
                    .FirstOrDefault()!.Id
            };

            var result = await _seasonService!.AddDriver(_seasonId, driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }*/

        [Test]
        public async Task DeleteResultSuccess()
        {
            Assert.IsNotEmpty(_context!.Results);

            var result = await _resultService!.DeleteResult(_resultId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Races);
        }

        [Test]
        public async Task DeleteResultWrongId()
        {
            var result = await _resultService!.DeleteResult(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }
    }
}
