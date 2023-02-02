using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit.User
{
    [TestFixture]
    public class RegistrationTests
    {
        private CarRacingTournamentDbContext? _context;
        private UserService? _userService;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_context, configuration);
        }

        [Test]
        public async Task Success()
        {
            var result = await _userService!.Registration(new RegistrationDto { Username = "username", Email = "test@test.com", Password = "Password1", PasswordAgain = "Password1" });
            Assert.IsTrue(result.IsSuccess);
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
            var result = await _userService!.Registration(new RegistrationDto { 
                Username = "", 
                Email = "test@test.com", 
                Password = "Password1", 
                PasswordAgain = "Password1" 
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task IncorrectUsername()
        {
            var result = await _userService!.Registration(new RegistrationDto { 
                Username = "user", 
                Email = "test@test.com", 
                Password = "Password1", 
                PasswordAgain = "Password1" 
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task MissingEmail()
        {
            var result = await _userService!.Registration(new RegistrationDto { 
                Username = "username", 
                Email = "", 
                Password = "Password1", 
                PasswordAgain = "Password1" 
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task IncorrectEmail()
        {
            var registrationDto = new RegistrationDto
            {
                Username = "username",
                Email = "test.com",
                Password = "Password1",
                PasswordAgain = "Password1"
            };
            var result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);

            registrationDto.Email = "test";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task MissingPassword()
        {
            var result = await _userService!.Registration(new RegistrationDto { 
                Username = "username", 
                Email = "test@test.com", 
                Password = "", 
                PasswordAgain = "Password1" 
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task IncorrectPassword()
        {
            var registrationDto = new RegistrationDto
            {
                Username = "username",
                Email = "test@test.com",
                Password = "password",
                PasswordAgain = "password"
            };
            var result = await _userService!.Registration(registrationDto);
            var valami = result;
            Assert.IsFalse(result.IsSuccess);

            registrationDto.Password = "Password";
            registrationDto.PasswordAgain = "Password";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);

            registrationDto.Password = "password1";
            registrationDto.PasswordAgain = "password1";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task NotEqualPassword()
        {
            var result = await _userService!.Registration(new RegistrationDto
            {
                Username = "username",
                Email = "test@test.com",
                Password = "Password",
                PasswordAgain = "Password1"
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task MissingPasswordAgain()
        {
            var result = await _userService!.Registration(new RegistrationDto
            {
                Username = "username",
                Email = "test@test.com",
                Password = "Password",
                PasswordAgain = ""
            });
            Assert.IsFalse(result.IsSuccess);
        }
    }
}
