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
    public class ResultTests
    {
        private ResultController? _resultController;
        private CarRacingTournamentDbContext? _context;
        private Result? _result;
        private SeasonService? _seasonService;
        private PermissionService? _permissionService;
        private DriverService? _driverService;
        private TeamService? _teamService;
        private ResultService? _resultService;
        private RaceService? _raceService;
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

            var seasonId = Guid.NewGuid();

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                SeasonId = seasonId
            };

            Season season = new Season
            {
                Id = seasonId,
                Name = "First Season",
                Drivers = new List<Driver>()
                {
                    new Driver
                    {
                        Id = Guid.NewGuid(),
                        Name = "SecondDriver",
                        Number = 2,
                        ActualTeam = team
                    },
                    new Driver {
                        Id = Guid.NewGuid(),
                        Name = "ThirdDriver",
                        Number = 3,
                        ActualTeam = team
                    }
                },
                Permissions = new List<Permission> {
                    new Permission {
                        Id = Guid.NewGuid(),
                        User = moderatorUser,
                        UserId = _moderatorUserId,
                        Type = PermissionType.Moderator
                    }
                }
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
                Team = team,
                Type = ResultType.Finished,
                Position = 3,
                Point = 15
            };

            _context.Results.Add(_result);
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
            _resultService = new ResultService(_context, mapper);
            _raceService = new RaceService(_context, mapper);

            _resultController = new ResultController(
                _resultService,
                _permissionService,
                _driverService,
                _teamService,
                _raceService,
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

            _resultController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task PostResultRaceNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _resultController!.PostResult(new ResultDto {
                Type = ResultType.Finished,
                Position = 2,
                Point = 18,
                DriverId = _context!.Drivers.Where(x => x.Name == "SecondDriver").First().Id,
                TeamId = _context.Teams.First().Id,
                RaceId = Guid.NewGuid()
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostResultForbid() {
            SetAuthentication(_anotherUserId);

            var result = await _resultController!.PostResult(new ResultDto {
                Type = ResultType.Finished,
                Position = 2,
                Point = 18,
                DriverId = _context!.Drivers.Where(x => x.Name == "SecondDriver").First().Id,
                TeamId = _context.Teams.First().Id,
                RaceId = _context.Races.First().Id
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PostResultArchived() {
            SetAuthentication(_moderatorUserId);

            _context!.Seasons.First().IsArchived = true;

            var result = await _resultController!.PostResult(new ResultDto {
                Type = ResultType.Finished,
                Position = 2,
                Point = 18,
                DriverId = _context!.Drivers.Where(x => x.Name == "SecondDriver").First().Id,
                TeamId = _context.Teams.First().Id,
                RaceId = _context.Races.First().Id
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
