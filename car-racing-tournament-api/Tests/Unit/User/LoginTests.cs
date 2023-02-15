using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit.User
{
    [TestFixture]
    public class LoginTests
    {
        private CarRacingTournamentDbContext? _context;
        private UserService? _userService;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _context.Users.Add(new Models.User { 
                Username = "username", 
                Email = "test@test.com", 
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG." 
            });
            _context.SaveChanges();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_context, _configuration);
        }
        
        [Test]
        public async Task Success()
        {
            var result = await _userService!.Login(new LoginDto { UsernameEmail = "username", Password = "Password1" });
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task MissingUsernameEmail()
        {
            var result = await _userService!.Login(new LoginDto { UsernameEmail = "", Password = "Password1" });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:LoginDetails"]);
        }

        [Test]
        public async Task IncorrectUsernameEmail()
        {
            var loginDto = new LoginDto { UsernameEmail = "user", Password = "Password1" };
            var result = await _userService!.Login(loginDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:LoginDetails"]);

            loginDto.UsernameEmail = "username";
            loginDto.Password = "password";
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:LoginDetails"]);
        }

        [Test]
        public async Task MissingPassword()
        {
            var result = await _userService!.Login(new LoginDto { UsernameEmail = "username", Password = "" });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:LoginDetails"]);
        }
    }
}
