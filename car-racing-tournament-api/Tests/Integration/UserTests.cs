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

            _user = new Models.User
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

        [Test]
        public async Task LoginFailed() {
            var result = await _userController!.Login(new LoginDto {
                UsernameEmail = "TestUser2",
                Password = "Valami1212"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    
        [Test]
        public async Task RegistrationFailed() {
            var result = await _userController!.Registration(new RegistrationDto {
                Username = "testUser",
                Email = "test@test.com",
                Password = "Valami",
                PasswordAgain = "Valami12"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetFailed() {
            SetAuthentication(Guid.NewGuid());

            var result = await _userController!.Get();

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutUserNotFound() {
            SetAuthentication(Guid.NewGuid());

            var result = await _userController!.Put(new UpdateUserDto {
                Username = "testUser1",
                Email = "test@test.com"
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutBadRequest() {
            SetAuthentication(_user!.Id);

            var result = await _userController!.Put(new UpdateUserDto {
                Username = "",
                Email = "test@test.com"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PutPasswordUserNotFound() {
            SetAuthentication(Guid.NewGuid());

            var result = await _userController!.Put(new UpdatePasswordDto {
                PasswordOld = "Valami12",
                Password = "Valami121",
                PasswordAgain = "Valami121"
            });

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task PutPasswordWrongOldPassword() {
            SetAuthentication(_user!.Id);

            var result = await _userController!.Put(new UpdatePasswordDto {
                PasswordOld = "Valami12",
                Password = "Valami121",
                PasswordAgain = "Valami121"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PutPasswordWrongNewPassword() {
            SetAuthentication(_user!.Id);

            var result = await _userController!.Put(new UpdatePasswordDto {
                PasswordOld = "Valami1212",
                Password = "Valami1212",
                PasswordAgain = "Valami121"
            });

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteUserNotFound() {
            SetAuthentication(Guid.NewGuid());

            var result = await _userController!.Delete();

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }
    }
}
