using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class SeasonTests
    {
        private CarRacingTournamentDbContext? _context;
        private SeasonService? _seasonService;
        private Guid _seasonId;
        private Guid _userId;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

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

            _seasonId = Guid.NewGuid();
            _context.Seasons.Add(new Season
            {
                Id = _seasonId,
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                UserSeasons = new List<UserSeason>()
                {
                    new UserSeason
                    {
                        Id = Guid.NewGuid(),
                        User = user,
                        UserId = _userId,
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
            });
            _context.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            var mapper = mockMapper.CreateMapper();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _seasonService = new SeasonService(_context, mapper, configuration);
        }

        [Test]
        public async Task GetSeasonsSuccess()
        {
            var result = await _seasonService!.GetSeasons();
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(result.Seasons!.Count, 1);

            //Assert.AreEqual(result.Seasons?.First().Teams.Count, 1); IT WILL BE IMPLEMENTED

            SeasonOutputDto season = result.Seasons![0];
            Assert.AreEqual(season.Id, _context!.Seasons.First().Id);
            Assert.AreEqual(season.Name, _context!.Seasons.First().Name);
            Assert.AreEqual(season.Description, _context!.Seasons.First().Description);
            Assert.AreEqual(season.IsArchived, _context!.Seasons.First().IsArchived);
        }

        [Test]
        public async Task GetSeasonByIdSuccess()
        {
            var result = await _seasonService!.GetSeasonById(_seasonId);
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

        // AddSeasonExists - NEED TO BE IMPLEMENTED

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
        public async Task AddSeasonIncorrectName()
        {
            var season = new SeasonCreateDto
            {
                Name = "test",
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

            var result = await _seasonService!.UpdateSeason(_seasonId, season);
            Assert.IsTrue(result.IsSuccess);

            var findSeason = _context!.Seasons.FirstAsync().Result;
            Assert.AreEqual(findSeason.Name, season.Name);
            Assert.AreEqual(findSeason.Description, season.Description);
            Assert.IsTrue(findSeason.IsArchived);
        }

        [Test]
        public async Task UpdateSeasonWrongId()
        {
            var season = new SeasonUpdateDto
            {
                Name = "test tournament",
                Description = "This is my modified tournament",
                IsArchived = true
            };

            var result = await _seasonService!.UpdateSeason(Guid.NewGuid(), season);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
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

            var result = await _seasonService!.UpdateSeason(_seasonId, season);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task UpdateSeasonIncorrectName()
        {
            var season = new SeasonUpdateDto
            {
                Name = "test",
                Description = "This is my second tournament",
                IsArchived = false
            };

            var result = await _seasonService!.UpdateSeason(_seasonId, season);
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task ArchiveSeasonSuccess()
        {
            Assert.IsFalse(_context!.Seasons.FirstAsync().Result.IsArchived);

            var result = await _seasonService!.ArchiveSeason(_seasonId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(_context.Seasons.FirstAsync().Result.IsArchived);

            result = await _seasonService!.ArchiveSeason(_seasonId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsFalse(_context.Seasons.FirstAsync().Result.IsArchived);
        }

        [Test]
        public async Task ArchiveSeasonWrongId()
        {
            var result = await _seasonService!.ArchiveSeason(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task DeleteSeasonSuccess()
        {
            Assert.IsNotEmpty(_context!.Seasons);

            var result = await _seasonService!.DeleteSeason(_seasonId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Seasons);
        }

        [Test]
        public async Task DeleteSeasonWrongId()
        {
            var result = await _seasonService!.DeleteSeason(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task GetSeasonsByUserIdSuccess()
        {
            var result = await _seasonService!.GetSeasonsByUserId(_userId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Seasons);
        }

        [Test]
        public async Task GetSeasonsByUserIdWrongId()
        {
            var result = await _seasonService!.GetSeasonsByUserId(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Seasons);
        }

        [Test]
        public async Task GetDriversBySeasonIdSuccess()
        {
            var result = await _seasonService!.GetDriversBySeasonId(_seasonId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Drivers);
            Assert.AreEqual(result.Drivers!.Count, 1);
        }

        [Test]
        public async Task GetDriversBySeasonIdWrongId()
        {
            var result = await _seasonService!.GetDriversBySeasonId(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Drivers);
        }

        [Test]
        public async Task AddDriverSuccess()
        {
            var season = _context!.Seasons.Where(x => x.Id == _seasonId).FirstOrDefault();
            var driverDto = new DriverDto
            {
                Name = "AddDriver1",
                RealName = "Add Driver",
                Number = 2,
                ActualTeamId = season!.Teams!.FirstOrDefault()!.Id
            };
            var result = await _seasonService!.AddDriver(season.Id, driverDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            driverDto.Name = "AddDriver2";
            driverDto.RealName = "";
            driverDto.Number = 3;
            result = await _seasonService!.AddDriver(season.Id, driverDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            driverDto.Name = "AddDriver3";
            driverDto.RealName = "Add Driver";
            driverDto.Number = 4;
            driverDto.ActualTeamId = null;
            result = await _seasonService!.AddDriver(season.Id, driverDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.AreEqual(_context.Seasons.FirstOrDefaultAsync().Result!.Drivers!.Count, 4);
            Assert.AreEqual(_context.Seasons.FirstOrDefaultAsync().Result!.Teams!.FirstOrDefault()!.Drivers!.Count, 3);
        }

        // AddDriverExists - NEED TO BE IMPLEMENTED

        [Test]
        public async Task AddDriverWrongSeasonId()
        {
            var driverDto = new DriverDto
            {
                Name = "AddDriver1",
                RealName = "Add Driver",
                Number = 2,
                ActualTeamId = null
            };
            var result = await _seasonService!.AddDriver(Guid.NewGuid(), driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task AddDriverMissingName()
        {
            var driverDto = new DriverDto
            {
                Name = "",
                RealName = "Add Driver",
                Number = 2,
                ActualTeamId = null
            };
            var result = await _seasonService!.AddDriver(_context!.Seasons.FirstOrDefaultAsync().Result!.Id, driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task AddDriverIncorrectNumber()
        {
            var driverDto = new DriverDto
            {
                Name = "NewDriver",
                RealName = "Add Driver",
                Number = -1,
                ActualTeamId = null
            };
            var result = await _seasonService!.AddDriver(_seasonId, driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            driverDto.Number = 0;
            result = await _seasonService!.AddDriver(_seasonId, driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            driverDto.Number = 100;
            result = await _seasonService!.AddDriver(_seasonId, driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task AddDriverWithAnotherSeasonTeam()
        {
            var anotherSeasonId = Guid.NewGuid();
            _context!.Seasons.Add(new Season
            {
                Id = anotherSeasonId,
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                UserSeasons = new List<UserSeason>()
                {
                    new UserSeason
                    {
                        Id = Guid.NewGuid(),
                        UserId = _userId,
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
                        Drivers = new List<Driver>(),
                        Results = new List<Result>(),
                        SeasonId = anotherSeasonId
                    }
                },
                Drivers = new List<Driver>(),
                Races = new List<Race>()
            });
            _context.SaveChanges();

            var driverDto = new DriverDto
            {
                Name = "AddDriver1",
                RealName = "Add Driver",
                Number = 2,
                ActualTeamId = _context.Seasons
                    .Where(x => x.Id == anotherSeasonId)
                    .FirstOrDefaultAsync().Result!.Teams!
                    .FirstOrDefault()!.Id
            };

            var result = await _seasonService!.AddDriver(_seasonId, driverDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task GetTeamsBySeasonIdSuccess()
        {
            var result = await _seasonService!.GetTeamsBySeasonId(_context!.Seasons.FirstOrDefaultAsync().Result!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Teams);
            Assert.AreEqual(result.Teams!.Count, 1);
        }

        [Test]
        public async Task GetTeamsBySeasonIdWrongId()
        {
            var result = await _seasonService!.GetDriversBySeasonId(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Drivers);
        }

        [Test]
        public async Task AddTeamSuccess()
        {
            var teamDto = new TeamDto
            {
                Name = "AddTeam1",
                Color = "123123"
            };
            var result = await _seasonService!.AddTeam(_seasonId, teamDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Seasons.FirstOrDefaultAsync().Result!.Teams!.Count, 2);

            teamDto.Color = "#123123";
            result = await _seasonService!.AddTeam(_seasonId, teamDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context.Seasons.FirstOrDefaultAsync().Result!.Teams!.Count, 3);
        }

        // teamAlreadyExists

        [Test]
        public async Task AddTeamMissingName()
        {
            var teamDto = new TeamDto
            {
                Name = "",
                Color = "123123"
            };
            var result = await _seasonService!.AddTeam(_seasonId, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task AddTeamIncorrectColorCode()
        {
            var teamDto = new TeamDto
            {
                Name = "AddTeam1",
                Color = "WRONGC"
            };
            var result = await _seasonService!.AddTeam(_seasonId, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            teamDto.Color = "#WRONGC";
            result = await _seasonService!.AddTeam(_seasonId, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            teamDto.Color = "QWEQWEWRONGC";
            result = await _seasonService!.AddTeam(_seasonId, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task GetRacesBySeasonIdSuccess()
        {
            var result = await _seasonService!.GetRacesBySeasonId(_context!.Seasons.FirstOrDefaultAsync().Result!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Races);
            Assert.AreEqual(result.Races!.Count, 1);
        }

        [Test]
        public async Task GetRacesBySeasonIdWrongId()
        {
            var result = await _seasonService!.GetRacesBySeasonId(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Races);
        }

        // Races exists

        [Test]
        public async Task AddRaceSuccess()
        {
            var raceDto = new RaceDto
            {
                Name = "Australian Grand Prix",
                DateTime = new DateTime(2023, 10, 10)
            };
            var result = await _seasonService!.AddRace(_seasonId, raceDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Seasons.FirstOrDefaultAsync().Result!.Races!.Count, 2);

            raceDto.DateTime = new DateTime();
            result = await _seasonService!.AddRace(_seasonId, raceDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context.Seasons.FirstOrDefaultAsync().Result!.Races!.Count, 3);
        }

        [Test]
        public async Task AddRaceMissingName()
        {
            var raceDto = new RaceDto
            {
                Name = "",
                DateTime = new DateTime(2023, 10, 10)
            };
            var result = await _seasonService!.AddRace(_seasonId, raceDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }
    }
}
