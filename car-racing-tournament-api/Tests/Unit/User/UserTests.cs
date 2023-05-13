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
        private IConfiguration? _configuration;

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

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userService = new UserService(_context);
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

        [Test]
        public async Task AlreadyExists()
        {
            var user = new Models.User { Username = "username1", Email = "test1@test.com", Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG." };
            _context!.Users.Add(user);
            _context.SaveChanges();

            var updateUserDto = new UpdateUserDto
            {
                Username = "username",
                Email = "test@test.com",
            };
            var result = await _userService!.UpdateUser(user, updateUserDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:UserNameExists"]);

            updateUserDto.Email = "test1@test.com";
            result = await _userService!.UpdateUser(user, updateUserDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:UserNameExists"]);

            updateUserDto.Username = "username1";
            updateUserDto.Email = "test@test.com";
            result = await _userService!.UpdateUser(user, updateUserDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailExists"]);
        }

        [Test]
        public async Task MissingUsername()
        {
            var result = await _userService!.UpdateUser(_user!, new UpdateUserDto
            {
                Username = "",
                Email = "test@test.com"
            });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:UserName"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:UserName"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailFormat"]);
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
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailFormat"]);

            registrationDto.Email = "test";
            result = await _userService!.UpdateUser(_user!, registrationDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:EmailFormat"]);
        }

        [Test]
        public async Task UpdatePasswordSuccess() {
            var result = await _userService!.UpdatePassword(_user!, new UpdatePasswordDto {
                PasswordOld = "Valami1212",
                Password = "Valami12121",
                PasswordAgain = "Valami12121"
            });
            
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task UpdateIncorrectPassword()
        {
            var updatePasswordDto =  new UpdatePasswordDto {
                PasswordOld = "Valami1212",
                Password = "password",
                PasswordAgain = "password"
            };

            var result = await _userService!.UpdatePassword(_user!, updatePasswordDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);

            updatePasswordDto.Password = "Password";
            updatePasswordDto.PasswordAgain = "Password";
            result = await _userService!.UpdatePassword(_user!, updatePasswordDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);

            updatePasswordDto.Password = "password1";
            updatePasswordDto.PasswordAgain = "password1";
            result = await _userService!.UpdatePassword(_user!, updatePasswordDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordFormat"]);
        }

        [Test]
        public async Task UpdateNotEqualPassword()
        {
            var updatePasswordDto =  new UpdatePasswordDto {
                PasswordOld = "Valami1212",
                Password = "Password1",
                PasswordAgain = "Password12"
            };
            var result = await _userService!.UpdatePassword(_user!, updatePasswordDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PasswordsPass"]);
        }

        [Test]
        public async Task DeleteUser() {
            var result = await _userService!.DeleteUser(_user!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
        }
    }
}
