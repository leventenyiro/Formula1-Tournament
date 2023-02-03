using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit.User
{
    [TestFixture]
    public class SeasonTests
    {
        private CarRacingTournamentDbContext? _context;
        private SeasonService? _seasonService;
        private Guid _id;
        private Guid _userId;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

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

            _id = Guid.NewGuid();
            _context.Seasons.Add(new Season { 
                Id = _id,
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                UserSeasons = new List<UserSeason>()
                {
                    new UserSeason
                    {
                        Id = Guid.NewGuid(),
                        User = user,
                        Permission = UserSeasonPermission.Admin
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
                Drivers = new List<Driver>(),
                Races = new List<Race>()
            });
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            _seasonService = new SeasonService(_context, mapper);
        }
        
        [Test]
        public async Task GetSeasonsSuccess()
        {
            var result = await _seasonService!.GetSeasons();
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(result.Seasons!.Count, 1);

            //Assert.AreEqual(result.Seasons?.First().Teams.Count, 1);

            SeasonOutputDto season = result.Seasons![0];
            Assert.AreEqual(season.Id, _context!.Seasons.First().Id);
            Assert.AreEqual(season.Name, _context!.Seasons.First().Name);
            Assert.AreEqual(season.Description, _context!.Seasons.First().Description);
            Assert.AreEqual(season.IsArchived, _context!.Seasons.First().IsArchived);
        }

        [Test]
        public async Task GetSeasonByIdSuccess()
        {
            var result = await _seasonService!.GetSeasonById(_id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            SeasonOutputDto season = result.Season!;
            Assert.AreEqual(season.Id, _context!.Seasons.First().Id);
            Assert.AreEqual(season.Name, _context!.Seasons.First().Name);
            Assert.AreEqual(season.Description, _context!.Seasons.First().Description);
            Assert.AreEqual(season.IsArchived, _context!.Seasons.First().IsArchived);
        }

        [Test]
        public async Task GetSeasonByIdNotFound()
        {
            var result = await _seasonService!.GetSeasonById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Season);
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
            Assert.AreEqual(findSeason.UserSeasons.Count, 1);
            Assert.AreEqual(findSeason.UserSeasons.First().UserId, _userId);
            Assert.AreEqual(findSeason.UserSeasons.First().Permission, UserSeasonPermission.Admin);
        }

        // AddSeasonExists

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
        }

        [Test]
        public async Task AddSeasonIncorrectName()
        {
            var season = new SeasonCreateDto
            {
                Name = "test",
                Description = "This is my second tournament"
            };

            var result = await _seasonService!.AddSeason(season, _userId);
            Assert.IsFalse(result.IsSuccess);
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

            var result = await _seasonService!.UpdateSeason(_id, season);
            Assert.IsTrue(result.IsSuccess);

            var findSeason = _context!.Seasons.FirstAsync().Result;
            Assert.AreEqual(findSeason.Name, season.Name);
            Assert.AreEqual(findSeason.Description, season.Description);
            Assert.IsTrue(findSeason.IsArchived);
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

            var result = await _seasonService!.UpdateSeason(_id, season);
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task UpdateSeasonIncorrectName()
        {
            var season = new SeasonUpdateDto
            {
                Name = "",
                Description = "This is my second tournament",
                IsArchived = false
            };

            var result = await _seasonService!.UpdateSeason(_id, season);
            Assert.IsFalse(result.IsSuccess);
        }

        // archive season
        [Test]
        public async Task ArchiveSeasonSuccess() {
            Assert.IsFalse(_context!.Seasons.FirstAsync().Result.IsArchived);

            var result = await _seasonService!.ArchiveSeason(_id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(_context.Seasons.FirstAsync().Result.IsArchived);

            result = await _seasonService!.ArchiveSeason(_id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(_context.Seasons.FirstAsync().Result.IsArchived);
        }

        // archive season with wrong id
        [Test]
        public async Task ArchiveSeasonWrongId()
        {
            var result = await _seasonService!.ArchiveSeason(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
        }

        // delete season

        // get seasons by userId
        // with wrong too
    }
}
