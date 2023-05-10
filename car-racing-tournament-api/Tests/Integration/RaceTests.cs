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
    public class RaceTests
    {
        private RaceController? _raceController;
        private CarRacingTournamentDbContext? _context;
        private Race? _race;
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

            _race = new Race
            {
                Id = Guid.NewGuid(),
                Name = "First Race",
                DateTime = DateTime.Now,
                Season = season
            };

            _context.Races.Add(_race!);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var seasonService = new SeasonService(_context, mapper);
            var permissionService = new PermissionService(_context);
            var driverService = new DriverService(_context, mapper);
            var teamService = new TeamService(_context);
            var resultService = new ResultService(_context, mapper);
            var raceService = new RaceService(_context, mapper);

            _raceController = new RaceController(
                raceService,
                permissionService,
                driverService,
                teamService,
                resultService,
                seasonService
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

            _raceController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task PutRaceAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _raceController!.Put(_race!.Id, new RaceDto {
                Name = "NewRace",
                DateTime = DateTime.Now
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PutRaceNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _raceController!.Put(Guid.NewGuid(), new RaceDto {
                Name = "NewRace",
                DateTime = DateTime.Now
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutRaceSeasonArchived() {
            SetAuthentication(_moderatorUserId);

            _race!.Season!.IsArchived = true;

            var result = await _raceController!.Put(_race!.Id, new RaceDto {
                Name = "NewRace",
                DateTime = DateTime.Now
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteRaceAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _raceController!.Delete(_race!.Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteRaceNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _raceController!.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteRaceArchived() {
            SetAuthentication(_moderatorUserId);

            _race!.Season!.IsArchived = true;

            var result = await _raceController!.Delete(_race!.Id);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
