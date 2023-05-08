using AutoMapper;
using car_racing_tournament_api.Controllers;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace car_racing_tournament_api.Tests.Integration
{
    [TestFixture]
    public class DriverTests
    {
        private DriverController? _driverController;
        private CarRacingTournamentDbContext? _context;
        private Driver? _driver;
        private SeasonService? _seasonService;
        private PermissionService? _permissionService;
        private DriverService? _driverService;
        private TeamService? _teamService;
        private IConfiguration? _configuration;
        private Guid _anotherUserId;
        private Guid _moderatorUserId;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _moderatorUserId = Guid.NewGuid();
            Models.User moderatorUser = new Models.User
            {
                Id = _moderatorUserId,
                Username = "TestUser2",
                Email = "test2@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };

            _anotherUserId = Guid.NewGuid();
            Models.User anotherUser = new Models.User
            {
                Id = _anotherUserId,
                Username = "TestUser3",
                Email = "test3@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };

            Season season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "First Season",
                Permissions = new List<Permission> {
                    new Permission {
                        Id = Guid.NewGuid(),
                        User = moderatorUser,
                        UserId = _moderatorUserId,
                        Type = PermissionType.Moderator
                    }
                }
            };

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                Season = season
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

            _seasonService = new SeasonService(_context, mapper);
            _permissionService = new PermissionService(_context);
            _driverService = new DriverService(_context, mapper);
            _teamService = new TeamService(_context);

            _driverController = new DriverController(
                _driverService,
                _permissionService,
                _teamService,
                _seasonService
            );
        }

        private void SetAuthentication(Guid? userId) {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            if (userId != null) {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userId.ToString()!) });
                var principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;
            }
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _driverController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task PutDriverSuccess() {
            SetAuthentication(_moderatorUserId);

            var result = await _driverController!.Put(_context!.Drivers.First().Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task PutDriverAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _driverController!.Put(_driver!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PutDriverNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _driverController!.Put(Guid.NewGuid(), new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutDriverTeamNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _driverController!.Put(_driver!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = Guid.NewGuid()
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutDriverSeasonArchived() {
            SetAuthentication(_moderatorUserId);

            _driver!.Season!.IsArchived = true;

            var result = await _driverController!.Put(_driver!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteDriverSuccess() {
            SetAuthentication(_moderatorUserId);

            var result = await _driverController!.Delete(_driver!.Id);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteDriverAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _driverController!.Delete(_driver!.Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteDriverNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _driverController!.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteSeasonArchived() {
            SetAuthentication(_moderatorUserId);

            _driver!.Season!.IsArchived = true;

            var result = await _driverController!.Delete(_driver!.Id);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
