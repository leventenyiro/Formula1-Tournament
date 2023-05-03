using car_racing_tournament_api.Data;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using car_racing_tournament_api.DTO;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    public class FavoriteTests
    {
        private CarRacingTournamentDbContext? _context;
        private FavoriteService? _favoriteService;
        private Models.User? _user1;
        private Season? _season1;
        private Season? _season2;
        private Favorite? _favorite;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _season1 = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season",
                IsArchived = false
            };
            _context.Seasons.Add(_season1);

            _user1 = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "FirstUser",
                Email = "first@user.com",
                Password = "test"
            };
            _context.Users.Add(_user1);

            _season2 = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season 2",
                IsArchived = false
            };
            _context.Seasons.Add(_season2);

            _favorite = new Favorite
            {
                Id = Guid.NewGuid(),
                Season = _season1,
                User = _user1
            };
            _context.Favorites.Add(_favorite);

            _context.SaveChanges();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _favoriteService = new FavoriteService(_context);
        }

        [Test]
        public async Task GetPermissionByIdSuccess() {
            var result = await _favoriteService!.GetFavoriteById(_favorite!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task GetPermissionByIdNotFound() {
            var result = await _favoriteService!.GetFavoriteById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:FavoriteNotFound"]);
        }

        [Test]
        public async Task AddFavoriteSuccess()
        {
            var result = await _favoriteService!.AddFavorite(_user1!.Id, _season2!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Favorites.Count(), 2);
        }

        [Test]
        public async Task AddPermissionExists() {
            var result = await _favoriteService!.AddFavorite(_user1!.Id, _season1!);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:FavoriteExists"]);
        }

        [Test]
        public async Task RemoveFavorite() {
            var result = await _favoriteService!.RemoveFavorite(_favorite!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Permissions.Count(), 0);
        }
    }
}
