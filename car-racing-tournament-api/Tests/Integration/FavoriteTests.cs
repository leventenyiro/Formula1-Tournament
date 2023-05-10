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
    public class FavoriteTests
    {
        private FavoriteController? _favoriteController;
        private CarRacingTournamentDbContext? _context;
        private Favorite? _favorite;
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

            _user1Id = Guid.NewGuid();
            Models.User user1 = new Models.User
            {
                Id = _user1Id,
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

            _context.Users.Add(user2);
            _context.SaveChanges();

            var seasonId = Guid.NewGuid();
            Season season = new Season
            {
                Id = seasonId,
                Name = "First Season"
            };

            _favorite = new Favorite {
                Id = Guid.NewGuid(),
                UserId = _user1Id,
                Season = season
            };

            _context.Favorites.Add(_favorite);
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var favoriteService = new FavoriteService(_context);
            var seasonService = new SeasonService(_context, mapper);
            var userService = new UserService(_context);

            _favoriteController = new FavoriteController(
                favoriteService,
                seasonService,
                userService
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

            _favoriteController!.ControllerContext.HttpContext = httpContext;
        }

        [Test]
        public async Task PostFavoriteSeasonNotFound() {
            SetAuthentication(_user2Id);

            var result = await _favoriteController!.Post(new FavoriteDto {
                UserId = _user2Id,
                SeasonId = Guid.NewGuid()
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostFavoriteUserNotFound() {
            SetAuthentication(_user2Id);

            var result = await _favoriteController!.Post(new FavoriteDto {
                UserId = Guid.NewGuid(),
                SeasonId = _favorite!.Season.Id
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PostFavoriteForbid() {
            SetAuthentication(_user1Id);

            var result = await _favoriteController!.Post(new FavoriteDto {
                UserId = _user2Id,
                SeasonId = _favorite!.Season.Id
            });

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteFavoriteNotFound() {
            SetAuthentication(_user1Id);

            var result = await _favoriteController!.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteFavoriteForbid() {
            SetAuthentication(_user2Id);

            var result = await _favoriteController!.Delete(_favorite!.Id);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }
    }
}
