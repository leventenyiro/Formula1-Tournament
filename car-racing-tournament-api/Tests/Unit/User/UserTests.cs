using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Security.Cryptography;

namespace car_racing_tournament_api.Tests.Unit.User
{
    [TestFixture]
    public class UserTests
    {
        private CarRacingTournamentDbContext? _context;
        private UserService? _userService;
        private Models.User? _user;

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
                Username = "username",
                Email = "test@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };
            _context.Users.Add(_user);
            _context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_context, configuration);
        }
        
        [Test]
        public async Task GetUserSuccess()
        {
            var result = await _userService!.GetUserById(_user!.Id);
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
        [Test]
        public async Task UpdateUserSuccess()
        {
            var updateUserDto = new UpdateUserDto
            {
                Username = "username",
                Email = "test@test.com"
            };
            var result = await _userService!.UpdateUser(_user!, updateUserDto);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(_context!.Users.Count(), 1);

            var user = _context.Users.FirstOrDefaultAsync();
            Assert.AreEqual(user.Result!.Username, updateUserDto.Username);
            Assert.AreEqual(user.Result!.Email, updateUserDto.Email);
        }

        /*[Test] IT WILL BE GOOD AFTER https://github.com/leventenyiro/car-racing-tournament/issues/85
        public async Task AlreadyExists()
        {
            _context!.Users.Add(new Models.User { Username = "username", Email = "test@test.com", Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG." });
            _context.SaveChanges();

            var registrationDto = new RegistrationDto
            {
                Username = "username",
                Email = "test@test.com",
                Password = "Password1",
                PasswordAgain = "Password1"
            };
            var result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);

            registrationDto.Email = "test1@test.com";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);

            registrationDto.Username = "username1";
            registrationDto.Email = "test@test.com";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);
        }*/

        [Test]
        public async Task MissingUsername()
        {
            var result = await _userService!.UpdateUser(_user!, new UpdateUserDto
            {
                Username = "",
                Email = "test@test.com"
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task IncorrectUsername()
        {
            var result = await _userService!.UpdateUser(_user!, new UpdateUserDto
            {
                Username = "user",
                Email = "test@test.com"
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task MissingEmail()
        {
            var result = await _userService!.UpdateUser(_user!, new UpdateUserDto
            {
                Username = "username",
                Email = ""
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task IncorrectEmail()
        {
            var registrationDto = new UpdateUserDto
            {
                Username = "username",
                Email = "test.com"
            };
            var result = await _userService!.UpdateUser(_user!, registrationDto);
            Assert.IsFalse(result.IsSuccess);

            registrationDto.Email = "test";
            result = await _userService!.UpdateUser(_user!, registrationDto);
            Assert.IsFalse(result.IsSuccess);
        }
    }
}
