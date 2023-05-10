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
    public class UserTests
    {
        private UserController? _userController;
        private CarRacingTournamentDbContext? _context;
        private User? _user;
        private IConfiguration? _configuration;
        private Guid _user1Id;
        private Guid _user2Id;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            Models.User _user = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "TestUser2",
                Email = "test2@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };
            
            _user2Id = Guid.NewGuid();
            Models.User user2 = new Models.User
            {
                Id = _user2Id,
                Username = "TestUser3",
                Email = "test3@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };

            _context.Users.Add(_user);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var userService = new UserService(_context);
            var permissionService = new PermissionService(_context);
            var seasonService = new SeasonService(_context, mapper);

            _userController = new UserController(
                userService,
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

            _userController!.ControllerContext.HttpContext = httpContext;
        }

        /*[Test]
        public async Task PostFavoriteSeasonNotFound() {
            SetAuthentication(_user2Id);

            var result = await _userController!.(new FavoriteDto {
                UserId = _user2Id,
                SeasonId = Guid.NewGuid()
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }*/
    }
}
