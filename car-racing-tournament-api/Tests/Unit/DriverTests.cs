using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class DriverTests
    {
        private CarRacingTournamentDbContext? _context;
        private DriverService? _driverService;
        private Driver? _driver;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            Season season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "First Season"
            };

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                Season = season
            };

            _driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = "FirstDriver",
                Number = 1,
                RealName = "First driver",
                ActualTeam = team,
                Season = season
            };

            _context.Drivers.Add(_driver);
            _context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _driverService = new DriverService(_context, configuration);
        }

        [Test]
        public async Task GetDriverByIdSuccess()
        {
            var result = await _driverService!.GetDriverById(_driver!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Driver driver = result.Driver!;
            Assert.AreEqual(driver.Id, _context!.Drivers.First().Id);
            Assert.AreEqual(driver.Name, _context!.Drivers.First().Name);
            Assert.AreEqual(driver.Number, _context!.Drivers.First().Number);
            Assert.AreEqual(driver.RealName, _context!.Drivers.First().RealName);
        }

        [Test]
        public async Task GetDriverByIdNotFound()
        {
            var result = await _driverService!.GetDriverById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.IsNull(result.Driver);
        }

        [Test]
        public async Task UpdateDriverSuccess()
        {
            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "New Team",
                Color = "456456",
                Season = _context!.Seasons.FirstOrDefaultAsync().Result!
            };

            _context.Teams.Add(team);
            _context.SaveChanges();

            var driverDto = new DriverDto
            {
                Name = "NewName",
                Number = 2,
                RealName = "New Name",
                ActualTeamId = team.Id
            };

            var result = await _driverService!.UpdateDriver(_driver!, driverDto, team);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            var findDriver = _context!.Drivers.FirstOrDefaultAsync().Result!;
            Assert.AreEqual(findDriver.Name, driverDto.Name);
            Assert.AreEqual(findDriver.Number, driverDto.Number);
            Assert.AreEqual(findDriver.RealName, driverDto.RealName);
            Assert.AreEqual(findDriver.ActualTeam!.Id, driverDto.ActualTeamId);

            driverDto.RealName = "";
            driverDto.ActualTeamId = null;
            result = await _driverService!.UpdateDriver(_driver!, driverDto, null!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.AreEqual(findDriver.RealName, driverDto.RealName);
            Assert.IsNull(findDriver.ActualTeamId);
        }

        // update - driver already exists

        [Test]
        public async Task UpdateDriverMissingName()
        {
            var driver = _context!.Drivers.Where(x => x.Number == 1).FirstOrDefaultAsync().Result;
            var driverDto = new DriverDto
            {
                Name = "",
                Number = 2,
                RealName = "New Name",
                ActualTeamId = driver!.ActualTeamId
            };

            var result = await _driverService!.UpdateDriver(driver, driverDto, driver.ActualTeam!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
            Assert.AreEqual(_context!.Drivers.ToListAsync().Result.Count, 1);
        }

        [Test]
        public async Task UpdateDriverIncorrectNumber()
        {
            var driverDto = new DriverDto
            {
                Name = "NewDriver",
                RealName = "New Driver",
                Number = -1,
                ActualTeamId = null
            };
            var result = await _driverService!.UpdateDriver(_driver!, driverDto, null!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            driverDto.Number = 0;
            result = await _driverService!.UpdateDriver(_driver!, driverDto, null!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);

            driverDto.Number = 100;
            result = await _driverService!.UpdateDriver(_driver!, driverDto, null!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task UpdateDriverWithAnotherSeasonTeam()
        {
            var anotherSeasonId = Guid.NewGuid();
            _context!.Seasons.Add(new Season
            {
                Id = anotherSeasonId,
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                Permissions = new List<Permission>()
                {
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
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

            var result = await _driverService!.UpdateDriver(_driver!, driverDto, _context.Seasons
                    .Where(x => x.Id == anotherSeasonId)
                    .FirstOrDefaultAsync().Result!.Teams!
                    .FirstOrDefault()!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task UpdateDriverTeamSuccess()
        {
            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "New Team",
                Color = "456456",
                Season = _context!.Seasons.FirstOrDefaultAsync().Result!
            };

            _context.Teams.Add(team);
            _context.SaveChanges();

            var result = await _driverService!.UpdateDriverTeam(_driver!, team);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            result = await _driverService!.UpdateDriverTeam(_driver!, null!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task UpdateDriverTeamWithAnotherSeasonTeam()
        {
            var anotherSeasonId = Guid.NewGuid();
            _context!.Seasons.Add(new Season
            {
                Id = anotherSeasonId,
                Name = "Test Season",
                Description = "This is our test season",
                IsArchived = false,
                Permissions = new List<Permission>()
                {
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
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
                        Drivers = new List<Driver>(),
                        Results = new List<Result>(),
                        SeasonId = anotherSeasonId
                    }
                },
                Drivers = new List<Driver>(),
                Races = new List<Race>()
            });
            _context.SaveChanges();

            var result = await _driverService!.UpdateDriverTeam(
                _driver!,
                _context.Seasons
                    .Where(x => x.Id == anotherSeasonId)
                    .FirstOrDefaultAsync().Result!.Teams!
                    .FirstOrDefault()!);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async Task DeleteResultSuccess()
        {
            Assert.IsNotEmpty(_context!.Drivers);

            var result = await _driverService!.DeleteDriver(_driver!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Drivers);
        }
    }
}
