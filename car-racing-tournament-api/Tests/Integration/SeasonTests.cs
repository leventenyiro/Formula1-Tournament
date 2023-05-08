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
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Security.Claims;
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
        private Guid _anotherUserId;
        private Guid _moderatorUserId;

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
            _context.Users.Add(anotherUser);
            _context.SaveChanges();

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
                    },
                    new Permission {
                        Id = Guid.NewGuid(),
                        User = moderatorUser,
                        UserId = _moderatorUserId,
                        Type = PermissionType.Moderator
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

        private void SetAuthentication(Guid? userId) {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            if (userId != null) {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userId.ToString()!) });
                var principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;
            }
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _seasonController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task GetSuccess()
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

        [Test]
        public async Task GetDetailsNotFound()
        {
            var result = await _seasonController!.GetDetails(Guid.NewGuid());
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostSeasonSuccess() {
            SetAuthentication(_userId);

            var result = await _seasonController!.Post(new SeasonCreateDto {
                Name = "test",
                Description = "adsad"
            });

            Assert.That(result, Is.TypeOf<ObjectResult>());
        }

        [Test]
        public async Task PutSeasonSuccess() {
            SetAuthentication(_userId);

            var result = await _seasonController!.Put(_season!.Id, new SeasonUpdateDto {
                Name = "test",
                Description = "adsad"
            });

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task PutSeasonAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _seasonController!.Put(_season!.Id, new SeasonUpdateDto {
                Name = "test",
                Description = "adsad"
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PutSeasonModerator() {
            SetAuthentication(_moderatorUserId);

            var result = await _seasonController!.Put(_season!.Id, new SeasonUpdateDto {
                Name = "test",
                Description = "adsad"
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PutSeasonNotFound() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.Put(Guid.NewGuid(), new SeasonUpdateDto {
                Name = "test",
                Description = "adsad"
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutSeasonArchived() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.Put(_season!.Id, new SeasonUpdateDto {
                Name = "test",
                Description = "adsad"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteSeasonSuccess() {
            SetAuthentication(_userId);

            var result = await _seasonController!.Delete(_season!.Id);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteSeasonAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _seasonController!.Delete(_season!.Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteSeasonModerator() {
            SetAuthentication(_moderatorUserId);

            var result = await _seasonController!.Delete(_season!.Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteSeasonNotFound() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteSeasonArchived() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.Delete(_season!.Id);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetByUserIdSuccess() {
            SetAuthentication(_userId);

            var result = await _seasonController!.GetByUserId();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task PostPermissionSuccess() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostPermission(_season!.Id, "TestUser3");
            
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task PostPermissionByEmailSuccess() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostPermission(_season!.Id, "test3@test.com");
            
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task PostPermissionSeasonNotFound() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostPermission(Guid.NewGuid(), "TestUser3");

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostPermissionUserNotFound() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostPermission(_season!.Id, "TestUser4");

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostPermissionAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _seasonController!.PostPermission(_season!.Id, "TestUser3");

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PostPermissionModerator() {
            SetAuthentication(_moderatorUserId);

            var result = await _seasonController!.PostPermission(_season!.Id, "TestUser3");

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PostPermissionArchived() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.PostPermission(_season!.Id, "TestUser3");

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostDriverSuccess() {
            SetAuthentication(_moderatorUserId);

            var result = await _seasonController!.PostDriver(_season!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });
            
            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task PostDriverSeasonNotFound() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostDriver(Guid.NewGuid(), new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostDriverTeamNotFound() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostDriver(_season!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = Guid.NewGuid()
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostDriverAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _seasonController!.PostDriver(_season!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PostDriverArchived() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.PostDriver(_season!.Id, new DriverDto {
                Name = "testDriver123",
                RealName = "",
                Number = 23,
                ActualTeamId = null
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostTeamSuccess() {
            SetAuthentication(_moderatorUserId);

            var result = await _seasonController!.PostTeam(_season!.Id, new TeamDto {
                Name = "testTeam23",
                Color = "#abcdef"
            });
            
            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task PostTeamSeasonNotFound() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostTeam(Guid.NewGuid(), new TeamDto {
                Name = "testTeam23",
                Color = "#abcdef"
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostTeamAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _seasonController!.PostTeam(_season!.Id, new TeamDto {
                Name = "testTeam23",
                Color = "#abcdef"
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PostTeamArchived() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.PostTeam(_season!.Id, new TeamDto {
                Name = "testTeam23",
                Color = "#abcdef"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostRaceSuccess() {
            SetAuthentication(_moderatorUserId);

            var result = await _seasonController!.PostRace(_season!.Id, new RaceDto {
                Name = "testTeam23",
                DateTime = DateTime.Now
            });
            
            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task PostRaceSeasonNotFound() {
            SetAuthentication(_userId);

            var result = await _seasonController!.PostRace(Guid.NewGuid(), new RaceDto {
                Name = "testTeam23",
                DateTime = DateTime.Now
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostRaceAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _seasonController!.PostRace(_season!.Id, new RaceDto {
                Name = "testTeam23",
                DateTime = DateTime.Now
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PostRaceArchived() {
            SetAuthentication(_userId);

            _season!.IsArchived = true;

            var result = await _seasonController!.PostRace(_season!.Id, new RaceDto {
                Name = "testTeam23",
                DateTime = DateTime.Now
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
