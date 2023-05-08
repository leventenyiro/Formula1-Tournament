using AutoMapper;
using car_racing_tournament_api.Controllers;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace car_racing_tournament_api.Tests.Integration
{
    [TestFixture]
    public class SeasonTests
    {
        private SeasonController? _seasonController;
        private CarRacingTournamentDbContext? _context;
        private Season? _season;
        private SeasonService? _seasonService;
        private UserService? _userService;
        private PermissionService? _permissionService;
        private DriverService? _driverService;
        private TeamService? _teamService;
        private RaceService? _raceService;
        private IConfiguration? _configuration;
        private Guid _userId;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _userId = Guid.NewGuid();

            Models.User user = new Models.User
            {
                Id = _userId,
                Username = "TestUser",
                Email = "test@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };

            Driver driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "TestDriver",
                RealName = "Test Driver",
                Number = 1,
                Results = new List<Result>()
            };

            _season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                Permissions = new List<Permission>()
                {
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        User = user,
                        UserId = _userId,
                        Type = PermissionType.Admin
                    }
                },
                Teams = new List<Team>()
                {
                    new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Team",
                        Color = "FF0000",
                        Drivers = new List<Driver>()
                        {
                            driver
                        },
                        Results = new List<Result>()
                    }
                },
                Drivers = new List<Driver>()
                {
                    driver
                },
                Races = new List<Race>()
                {
                    new Race
                    {
                        Id = Guid.NewGuid(),
                        Name = "First Race",
                        DateTime = new DateTime(2023, 1, 1)
                    }
                }
            };

            _context.Seasons.Add(_season);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            
            _seasonService = new SeasonService(_context, mapper);
            _permissionService = new PermissionService(_context);
            _userService = new UserService(_context);
            _driverService = new DriverService(_context, mapper);
            _teamService = new TeamService(_context);
            _raceService = new RaceService(_context, mapper);

            _seasonController = new SeasonController(
                _seasonService,
                _permissionService,
                _userService,
                _driverService,
                _teamService,
                _raceService
            );
        }

        [Test]
        public async Task GetSeasonsSuccess()
        {
            var result = await _seasonController!.Get();
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task GetDetailsSuccess()
        {
            var result = await _seasonController!.GetDetails(_season!.Id);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}
