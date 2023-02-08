using System.Numerics;
using AutoMapper;
using car_racing_tournament_api.Data;
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
        
        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            Race race = new Race
            {
                Id = Guid.NewGuid(),
                Name = "My first race",
                DateTime = new DateTime(2023, 1, 1, 18, 0, 0),
                Results = new List‹Result›() {
                    new Result 
                }
            };

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _raceService = new RaceService(_context, mapper);
        }

        // getracebyid

        // update, delete race

        // results by raceId

        // add result by raceId
    }
}
