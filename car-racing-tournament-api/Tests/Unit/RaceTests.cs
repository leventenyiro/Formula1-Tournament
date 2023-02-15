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
    public class RaceTests
    {
        private CarRacingTournamentDbContext? _context;
        private RaceService? _raceService;
        private Race? _race;
        
        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First team",
                Color = "123123"
            };

            _race = new Race
            {
                Id = Guid.NewGuid(),
                Name = "My first race",
                DateTime = new DateTime(2023, 1, 1, 18, 0, 0),
                Results = new List<Result>
                {
                    new Result
                    {
                        Id = Guid.NewGuid(),
                        Driver = new Driver
                        {
                            Id = Guid.NewGuid(),
                            Name = "FirstDriver",
                            RealName = "First Driver",
                            Number = 1,
                            ActualTeam = team
                        },
                        Points = 25,
                        Position = 1,
                        Team = team
                    }
                },
                Season = new Season
                {
                    Id = Guid.NewGuid(),
                    Name = "First season",
                    IsArchived = false,
                    Drivers = new List<Driver>
                    {
                        new Driver
                        {
                            Id = Guid.NewGuid(),
                            Name = "SecondDriver",
                            Number = 2,
                            ActualTeam = team
                        }
                    },
                    Teams = new List<Team> { team }
                }
            };

            _context.Races.Add(_race);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _raceService = new RaceService(_context, mapper, configuration);
        }

        [Test]
        public async Task GetRaceByIdSuccess()
        {
            var result = await _raceService!.GetRaceById(_race!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Race race = result.Race!;
            Assert.AreEqual(race.Id, _context!.Races.First().Id);
            Assert.AreEqual(race.Name, _context!.Races.First().Name);
            Assert.AreEqual(race.DateTime, _context!.Races.First().DateTime);
        }

        [Test]
        public async Task GetRaceByIdNotFound()
        {
            var result = await _raceService!.GetRaceById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Race);
        }

        [Test]
        public async Task UpdateRaceSuccess()
        {
            var race = new RaceDto
            {
                Name = "test tournament",
                DateTime = new DateTime(2022, 12, 12, 12, 0, 0)
            };

            var result = await _raceService!.UpdateRace(_race!, race);
            Assert.IsTrue(result.IsSuccess);

            var findRace = _context!.Races.FirstAsync().Result;
            Assert.AreEqual(findRace.Name, race.Name);
            Assert.AreEqual(findRace.DateTime, race.DateTime);
        }

        [Test]
        public async Task UpdateRaceMissingName()
        {
            var race = new RaceDto
            {
                Name = "",
                DateTime = new DateTime(2022, 12, 12, 12, 0, 0)
            };

            var result = await _raceService!.UpdateRace(_race!, race);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task DeleteRaceSuccess()
        {
            Assert.IsNotEmpty(_context!.Races);

            var result = await _raceService!.DeleteRace(_race!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Races);
        }

        /*[Test]
        public async Task GetResultsByRaceIdSuccess()
        {
            var result = await _resultService!.GetResultsByRaceId(_race!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Results);
            Assert.AreEqual(result.Results!.Count, 1);
        }

        [Test]
        public async Task AddResultSuccess()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = 18,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.AddResult(_race!, resultDto, driver, driver.ActualTeam!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 2);
        }

        //[Test]
        //public async Task AddResultResultExists() // a driver cannot reach more result on a race

        [Test]
        public async Task AddResultNotPositivePosition()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = 18,
                Position = -1,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };

            var result = await _resultService!.AddResult(_race!, resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);

            resultDto.Position = 0;
            result = await _resultService!.AddResult(_race!, resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task AddResultMinusPoints()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 2).FirstOrDefaultAsync().Result;
            var resultDto = new ResultDto
            {
                Points = -2,
                Position = 2,
                DriverId = driver!.Id,
                TeamId = (Guid)driver.ActualTeamId!
            };
            
            var result = await _resultService!.AddResult(_race!, resultDto, driver, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Results.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task AddResultWithAnotherSeason()
        {
            var anotherSeasonId = Guid.NewGuid();
            _context!.Races.Add(new Race
            {
                Id = Guid.NewGuid(),
                Name = "Another race",
                SeasonId = anotherSeasonId
            });
            _context.SaveChanges();

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
                DriverId = driver.Id,
                TeamId = _context.Teams.FirstOrDefaultAsync().Result!.Id,
            };

            var result = await _resultService!.AddResult(_race!, resultDto, driver, _context.Teams.FirstOrDefaultAsync().Result!);
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

            result = await _resultService!.AddResult(_race!, resultDto, _context.Drivers.FirstOrDefaultAsync().Result!, team);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }*/
    }
}
