using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Xml.Linq;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class SeasonTests
    {
        private CarRacingTournamentDbContext? _context;
        private SeasonService? _seasonService;
        private Season? _season;
        private Guid _userId;
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
            _context.Database.EnsureCreated();

            _userId = Guid.NewGuid();

            Models.User user = new Models.User
            {
                Id = _userId,
                Username = "TestUser",
                Email = "test@test.com",
                Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG."
            };

            Driver driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "TestDriver",
                RealName = "Test Driver",
                Number = 1,
                Results = new List<Result>()
            };

            _season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                Permissions = new List<Permission>()
                {
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        User = user,
                        UserId = _userId,
                        Type = PermissionType.Admin
                    }
                },
                Teams = new List<Team>()
                {
                    new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Team",
                        Color = "FF0000",
                        Drivers = new List<Driver>()
                        {
                            driver
                        },
                        Results = new List<Result>()
                    }
                },
                Drivers = new List<Driver>()
                {
                    driver
                },
                Races = new List<Race>()
                {
                    new Race
                    {
                        Id = Guid.NewGuid(),
                        Name = "First Race",
                        DateTime = new DateTime(2023, 1, 1)
                    }
                }
            };

            _context.Seasons.Add(_season);
            _context.SaveChangesAsync();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _seasonService = new SeasonService(_context, mapper, _configuration);
        }

        [Test]
        public async Task GetSeasonsSuccess()
        {
            var result = await _seasonService!.GetSeasons();
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(result.Seasons!.Count, 1);

            //Assert.AreEqual(result.Seasons?.First().Teams.Count, 1); IT WILL BE IMPLEMENTED - but we need to edit outputDto

            SeasonOutputDto season = result.Seasons![0];
            Assert.AreEqual(season.Id, _context!.Seasons.First().Id);
            Assert.AreEqual(season.Name, _context!.Seasons.First().Name);
            Assert.AreEqual(season.Description, _context!.Seasons.First().Description);
            Assert.AreEqual(season.IsArchived, _context!.Seasons.First().IsArchived);
        }

        [Test]
        public async Task GetSeasonByIdSuccess()
        {
            var result = await _seasonService!.GetSeasonById(_season!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Season season = result.Season!;
            Assert.AreEqual(season.Id, _context!.Seasons.First().Id);
            Assert.AreEqual(season.Name, _context!.Seasons.First().Name);
            Assert.AreEqual(season.Description, _context!.Seasons.First().Description);
            Assert.AreEqual(season.IsArchived, _context!.Seasons.First().IsArchived);
        }

        [Test]
        public async Task GetSeasonByIdWithDetailsSuccess()
        {
            var result = await _seasonService!.GetSeasonByIdWithDetails(_season!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            SeasonOutputDto season = result.Season!;
            Assert.AreEqual(season.Id, _context!.Seasons.First().Id);
            Assert.AreEqual(season.Name, _context!.Seasons.First().Name);
            Assert.AreEqual(season.Description, _context!.Seasons.First().Description);
            Assert.AreEqual(season.IsArchived, _context!.Seasons.First().IsArchived);
        }

        [Test]
        public async Task AddSeasonSuccess()
        {
            var season = new SeasonCreateDto
            {
                Name = "Second tournament",
                Description = "This is my second tournament"
            };
            var result = await _seasonService!.AddSeason(season, _userId);
            Assert.IsTrue(result.IsSuccess);

            var findSeason = _context!.Seasons.Where(x => x.Name == season.Name).FirstOrDefaultAsync().Result!;
            Assert.AreEqual(findSeason.Name, season.Name);
            Assert.AreEqual(findSeason.Description, season.Description);
            Assert.IsFalse(findSeason.IsArchived);
            Assert.IsNull(findSeason.Teams);
            Assert.IsNull(findSeason.Drivers);
            Assert.IsNull(findSeason.Races);
        }

        [Test]
        public async Task AddSeasonExists()
        {
            var season = new SeasonCreateDto
            {
                Name = "Test Season",
                Description = "This is our test season"
            };

            var result = await _seasonService!.AddSeason(season, _userId);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration?["ErrorMessage:SeasonExists"]);
        }

        [Test]
        public async Task AddSeasonMissingName()
        {
            var season = new SeasonCreateDto
            {
                Name = "",
                Description = "This is my second tournament"
            };

            var result = await _seasonService!.AddSeason(season, _userId);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task UpdateSeasonSuccess()
        {
            var season = new SeasonUpdateDto
            {
                Name = "test tournament",
                Description = "This is my modified tournament",
                IsArchived = true
            };

            var result = await _seasonService!.UpdateSeason(_season!, season);
            Assert.IsTrue(result.IsSuccess);

            var findSeason = _context!.Seasons.FirstAsync().Result;
            Assert.AreEqual(findSeason.Name, season.Name);
            Assert.AreEqual(findSeason.Description, season.Description);
            Assert.IsTrue(findSeason.IsArchived);
        }

        [Test]
        public async Task UpdateSeasonExists()
        {
            var season = new SeasonUpdateDto
            {
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false
            };

            var result = await _seasonService!.UpdateSeason(_context!.Seasons.First(), season);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration?["ErrorMessage:SeasonExists"]);
        }

        [Test]
        public async Task UpdateSeasonMissingName()
        {
            var season = new SeasonUpdateDto
            {
                Name = "",
                Description = "This is my second tournament",
                IsArchived = false
            };

            var result = await _seasonService!.UpdateSeason(_season!, season);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task ArchiveSeasonSuccess()
        {
            Assert.IsFalse(_context!.Seasons.FirstAsync().Result.IsArchived);

            var result = await _seasonService!.ArchiveSeason(_season!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(_context.Seasons.FirstAsync().Result.IsArchived);

            result = await _seasonService!.ArchiveSeason(_season!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsFalse(_context.Seasons.FirstAsync().Result.IsArchived);
        }

        [Test]
        public async Task DeleteSeasonSuccess()
        {
            Assert.IsNotEmpty(_context!.Seasons);
            Assert.IsNotEmpty(_context.Permissions);

            var result = await _seasonService!.DeleteSeason(_season!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Seasons);
            Assert.IsEmpty(_context.Permissions);
        }

        [Test]
        public async Task GetSeasonsByUserIdSuccess()
        {
            var result = await _seasonService!.GetSeasonsByUserId(_userId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Seasons);
        }
    }
}
