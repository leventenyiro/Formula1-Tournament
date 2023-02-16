using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit.User
{
    [TestFixture]
    public class RegistrationTests
    {
        private CarRacingTournamentDbContext? _context;
        private UserService? _userService;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseSqlite(connection)
                .Options;

            _context = new CarRacingTournamentDbContext(options);
            _context.Database.EnsureCreatedAsync();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_context, _configuration);
        }

        [Test]
        public async Task Success()
        {
            Assert.AreEqual(_context!.Users.CountAsync(), 0);

            var registrationDto = new RegistrationDto { 
                Username = "username", 
                Email = "test@test.com", 
                Password = "Password1", 
                PasswordAgain = "Password1" 
            };
            var result = await _userService!.Registration(registrationDto);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(_context!.Users.CountAsync(), 1);
            
            var user = _context.Users.FirstOrDefaultAsync();
            Assert.AreEqual(user.Result!.Username, registrationDto.Username);
            Assert.AreEqual(user.Result!.Email, registrationDto.Email);
            Assert.AreNotEqual(user.Result!.Username, registrationDto.Password);

            registrationDto.Username = "   username2";
            registrationDto.Email = "   test2@test.com";
            result = await _userService!.Registration(registrationDto);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(_context!.Users.CountAsync(), 2);

            Assert.IsNotNull(_context.Users.Where(x => x.Username == "username2" && x.Email == "test2@test.com").FirstOrDefaultAsync());
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:UserName"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:UserName"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailFormat"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailFormat"]);

            registrationDto.Email = "test";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailFormat"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);

            registrationDto.Password = "Password";
            registrationDto.PasswordAgain = "Password";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);

            registrationDto.Password = "password1";
            registrationDto.PasswordAgain = "password1";
            result = await _userService!.Registration(registrationDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);
        }

        [Test]
        public async Task NotEqualPassword()
        {
            var result = await _userService!.Registration(new RegistrationDto
            {
                Username = "username",
                Email = "test@test.com",
                Password = "Password1",
                PasswordAgain = "Password12"
            });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordsPass"]);
        }

        [Test]
        public async Task MissingPasswordAgain()
        {
            var result = await _userService!.Registration(new RegistrationDto
            {
                Username = "username",
                Email = "test@test.com",
                Password = "Password1",
                PasswordAgain = ""
            });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordsPass"]);
        }
    }
}
