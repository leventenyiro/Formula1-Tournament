using AutoMapper;
using car_racing_tournament_api.Controllers;
using car_racing_tournament_api.Data;
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
    public class PermissionTests
    {
        private PermissionController? _permissionController;
        private CarRacingTournamentDbContext? _context;
        private IConfiguration? _configuration;
        private Guid _adminUserId;
        private Guid _anotherUserId;
        private Guid _moderatorUserId;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _adminUserId = Guid.NewGuid();
            Models.User adminUser = new Models.User
            {
                Id = _adminUserId,
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

            Season season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "First Season",
                Permissions = new List<Permission> {
                    new Permission {
                        Id = Guid.NewGuid(),
                        User = adminUser,
                        UserId = _adminUserId,
                        Type = PermissionType.Admin
                    },
                    new Permission {
                        Id = Guid.NewGuid(),
                        User = moderatorUser,
                        UserId = _moderatorUserId,
                        Type = PermissionType.Moderator
                    }
                },
            };

            _context.Seasons.Add(season!);
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

            _permissionController = new PermissionController(
                permissionService,
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

            _permissionController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task UpgradePermissionSuccess() {
            SetAuthentication(_adminUserId);

            var result = await _permissionController!.UpgradePermission(_context!.Permissions.Where(x => x.Type == PermissionType.Moderator).First().Id);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task UpgradePermissionNotFound()
        {
            SetAuthentication(_adminUserId);

            var result = await _permissionController!.UpgradePermission(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpgradePermissionForbid()
        {
            SetAuthentication(_moderatorUserId);

            var result = await _permissionController!.UpgradePermission(_context!.Permissions.Where(x => x.Type == PermissionType.Moderator).First().Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeletePermissionSuccess()
        {
            SetAuthentication(_adminUserId);

            var result = await _permissionController!.Delete(_context!.Permissions.Where(x => x.Type == PermissionType.Moderator).First().Id);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task DeletePermissionNotFound()
        {
            SetAuthentication(_adminUserId);

            var result = await _permissionController!.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeletePermissionForbid()
        {
            SetAuthentication(_moderatorUserId);

            var result = await _permissionController!.Delete(_context!.Permissions.Where(x => x.Type == PermissionType.Admin).First().Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }
    }
}
