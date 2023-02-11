using car_racing_tournament_api.Data;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    public class UserSeasonTests
    {
        private CarRacingTournamentDbContext? _context;
        private UserSeasonService? _userSeasonService;
        private Guid _user1Id;
        private Guid _user2Id;
        private Guid _season1Id;
        private Guid _season2Id;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _season1Id = Guid.NewGuid();
            var season1 = new Season
            {
                Id = _season1Id,
                Name = "Test Season",
                IsArchived = false
            };
            _context.Seasons.Add(season1);

            _user1Id = Guid.NewGuid();
            var user1 = new Models.User
            {
                Id = _user1Id,
                Username = "FirstUser",
                Email = "first@user.com",
                Password = "test"
            };
            _context.Users.Add(user1);

            _season2Id = Guid.NewGuid();
            var season2 = new Season
            {
                Id = _season2Id,
                Name = "Test Season 2",
                IsArchived = false
            };
            _context.Seasons.Add(season2);

            _user2Id = Guid.NewGuid();
            var user2 = new Models.User
            {
                Id = _user2Id,
                Username = "SecondUser",
                Email = "second@user.com",
                Password = "test"
            };
            _context.Users.Add(user2);

            _context.UserSeasons.Add(new UserSeason
            {
                Id = Guid.NewGuid(),
                Season = season1,
                User = user1,
                Permission = UserSeasonPermission.Admin
            });

            _context.UserSeasons.Add(new UserSeason
            {
                Id = Guid.NewGuid(),
                Season = season1,
                User = user2,
                Permission = UserSeasonPermission.Moderator
            });

            _context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _userSeasonService = new UserSeasonService(_context, configuration);
        }

        [Test]
        public async Task IsAdminSuccess()
        {
            var result = await _userSeasonService!.IsAdmin(_user1Id, _season1Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsAdminFailed()
        {
            var result = await _userSeasonService!.IsAdmin(_user2Id, _season1Id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsAdminModeratorSuccess()
        {
            var result = await _userSeasonService!.IsAdminModerator(_user1Id, _season1Id);
            Assert.IsTrue(result);

            result = await _userSeasonService!.IsAdminModerator(_user2Id, _season1Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsAdminModeratorFailed()
        {
            var result = await _userSeasonService!.IsAdmin(_user1Id, _season2Id);
            Assert.IsFalse(result);

            result = await _userSeasonService!.IsAdmin(_user2Id, _season2Id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddAdminSuccess()
        {
            var result = await _userSeasonService!.AddAdmin(_user1Id, _season2Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
        }

        /*[Test]
        public async Task AddAdminFailed()
        {
            var user = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "ThirdUser",
                Email = "third@user.com",
                Password = "test"
            };

            var result = await _userSeasonService!.AddAdmin(user.Id, _season1Id);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task AddModeratorSuccess()
        {
            var result = await _userSeasonService!.AddModerator(_user1Id, _season2Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task AddModeratorFailed()
        {
            var user = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "ThirdUser",
                Email = "third@user.com",
                Password = "test"
            };

            var result = await _userSeasonService!.AddModerator(user.Id, _season1Id);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }*/
    }
}
