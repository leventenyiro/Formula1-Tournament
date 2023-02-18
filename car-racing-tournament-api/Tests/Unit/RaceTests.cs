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
        private IConfiguration? _configuration;
        
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
                        Point = 25,
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

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _raceService = new RaceService(_context, mapper, _configuration);
        }

        [Test]
        public async Task GetRacesBySeasonSuccess()
        {
            var result = await _raceService!.GetRacesBySeason(_context!.Seasons.First());
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Races);
            Assert.AreEqual(result.Races!.Count, 1);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceNotFound"]);
            Assert.IsNull(result.Race);
        }

        [Test]
        public async Task AddRaceSuccess()
        {
            var raceDto = new RaceDto
            {
                Name = "Australian Grand Prix",
                DateTime = new DateTime(2023, 10, 10)
            };

            var season = _context!.Seasons.First();

            var result = await _raceService!.AddRace(season, raceDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Seasons.FirstOrDefaultAsync().Result!.Races!.Count, 2);

            raceDto.Name = "Another race";
            raceDto.DateTime = new DateTime();
            result = await _raceService!.AddRace(season, raceDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context.Seasons.FirstOrDefaultAsync().Result!.Races!.Count, 3);
        }

        [Test]
        public async Task AddRaceMissingName()
        {
            var raceDto = new RaceDto
            {
                Name = "",
                DateTime = new DateTime(2023, 10, 10)
            };
            var result = await _raceService!.AddRace(_context!.Seasons.First(), raceDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceName"]);
        }

        [Test]
        public async Task AddRaceExists() {
            var season = _context!.Seasons.First();

            var result = await _raceService!.AddRace(season, new RaceDto() {
                Name = "My first race"
            });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceNameExists"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceName"]);
        }

        [Test]
        public async Task UpdateRaceExists() {
            var season = _context!.Seasons.First();

            var race = new Race {
                Id = Guid.NewGuid(),
                Name = "Second Race",
                SeasonId = season.Id
            };

            var raceDto = new RaceDto() {
                Name = "My first race"
            };

            var result = await _raceService!.UpdateRace(race, raceDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:RaceNameExists"]);
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
    }
}
