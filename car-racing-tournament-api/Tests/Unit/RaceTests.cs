using System.Numerics;
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
        private Guid _raceId;
        
        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _raceId = Guid.NewGuid();

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First team",
                Color = "123123"
            };

            _context.Races.Add(new Race
            {
                Id = _raceId,
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

                }
            });
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _raceService = new RaceService(_context, mapper);
        }

        [Test]
        public async Task GetRaceByIdSuccess()
        {
            var result = await _raceService!.GetRaceById(_raceId);
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

            var result = await _raceService!.UpdateRace(_raceId, race);
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

            var result = await _raceService!.UpdateRace(_raceId, race);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        // results by raceId
        [Test]
        public async Task GetResultsByRaceIdSuccess()
        {
            var result = await _raceService!.GetResultsByRaceId(_raceId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Results!);
        }

        [Test]
        public async Task GetResultsByRaceIdWrongId()
        {
            var result = await _raceService!.GetResultsByRaceId(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Results!);
        }

        // add result by raceId
    }
}
