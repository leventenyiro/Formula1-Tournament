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
    public class TeamTests
    {
        private TeamController? _teamController;
        private CarRacingTournamentDbContext? _context;
        private Team? _team;
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

            _team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                Season = season
            };

            _context.Teams.Add(_team!);
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

            _teamController = new TeamController(
                teamService,
                permissionService,
                seasonService,
                driverService,
                resultService
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

            _teamController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task PutTeamAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _teamController!.Put(_team!.Id, new TeamDto {
                Name = "testDriver123",
                Color = "#abcdef"
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task PutTeamNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _teamController!.Put(Guid.NewGuid(), new TeamDto {
                Name = "testDriver123",
                Color = "#abcdef"
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutTeamSeasonArchived() {
            SetAuthentication(_moderatorUserId);

            _team!.Season!.IsArchived = true;

            var result = await _teamController!.Put(_team!.Id, new TeamDto {
                Name = "testDriver123",
                Color = "#abcdef"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteTeamAnotherUser() {
            SetAuthentication(_anotherUserId);

            var result = await _teamController!.Delete(_team!.Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteTeamNotFound() {
            SetAuthentication(_moderatorUserId);

            var result = await _teamController!.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteTeamArchived() {
            SetAuthentication(_moderatorUserId);

            _team!.Season!.IsArchived = true;

            var result = await _teamController!.Delete(_team!.Id);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
