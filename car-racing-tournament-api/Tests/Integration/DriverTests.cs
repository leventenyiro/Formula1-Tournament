using AutoMapper;
using car_racing_tournament_api.Controllers;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace car_racing_tournament_api.Tests.Integration
{
    [TestFixture]
    public class DriverTests
    {
        private DriverController _driverController;
        private Mock<IDriver> _mockDriverService;
        private Mock<IPermission> _mockPermissionService;
        private Mock<ITeam> _mockTeamService;
        private Mock<ISeason> _mockSeasonService;
        private CarRacingTournamentDbContext? _context;
        private Driver? _driver;
        private DriverService? _driverService;
        private PermissionService? _permissionService;
        private TeamService? _teamService;
        private SeasonService? _seasonService;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            // Create mock services
            _mockDriverService = new Mock<IDriver>();
            _mockPermissionService = new Mock<IPermission>();
            _mockTeamService = new Mock<ITeam>();
            _mockSeasonService = new Mock<ISeason>();

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

            Permission permission = new Permission
            {
                Id = new Guid(),
                SeasonId = season.Id,
                UserId = new Guid()
            };

            _driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "FirstDriver",
                Number = 1,
                RealName = "First driver",
                ActualTeam = team,
                Season = season
            };

            _context.Drivers.Add(_driver);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _driverService = new DriverService(_context, mapper);
            _permissionService = new PermissionService(_context);
            _teamService = new TeamService(_context);
            _seasonService = new SeasonService(_context, mapper);

            _driverController = new DriverController(_driverService, _permissionService, _teamService, _seasonService);
        }

        [Test]
        public async Task PutDriverOk()
        {
            // Arrange
            var driverDto = new DriverDto { 
                Name = _driver!.Name,
                RealName= _driver.RealName,
                Number = _driver.Number,
                ActualTeamId = _driver.ActualTeamId,
            };

            // Act
            var result = await _driverController.Put(_driver.Id, driverDto);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }
}
