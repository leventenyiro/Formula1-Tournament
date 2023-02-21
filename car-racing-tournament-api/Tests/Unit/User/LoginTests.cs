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
        public void Success()
        {
            var result = _userService!.Login(_context!.Users.First(), "Password1", false);
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void MissingPassword()
        {
            var result = _userService!.Login(_context!.Users.First(), "", false);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:LoginDetails"]);
        }
    }
}
