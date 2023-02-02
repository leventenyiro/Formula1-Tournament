using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit.User
{
    [TestFixture]
    public class UserTests
    {
        private CarRacingTournamentDbContext? _context;
        private UserService? _userService;
        private Guid _id;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _id = Guid.NewGuid();
            _context.Users.Add(new Models.User { Id = _id, Username = "username", Email = "test@test.com", Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG." });
            _context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_context, configuration);
        }
        
        [Test]
        public async Task GetUserSuccess()
        {
            var result = await _userService!.GetUser(_id.ToString());
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.User!.Username, "username");
            Assert.AreEqual(result.User!.Email, "test@test.com");
        }

        [Test]
        public async Task GetUserByUsernameEmailSuccess()
        {
            var result = await _userService!.GetUserByUsernameEmail("username");
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.User!.Username, "username");
            Assert.AreEqual(result.User!.Email, "test@test.com");

            result = await _userService!.GetUserByUsernameEmail("test@test.com");
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.User!.Username, "username");
            Assert.AreEqual(result.User!.Email, "test@test.com");
        }

        // UpdateUser, UpdatePassword
    }
}
