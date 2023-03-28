using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class TeamTests
    {
        private CarRacingTournamentDbContext? _context;
        private TeamService? _teamService;
        private Team? _team;
        private IConfiguration? _configuration;

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

            _team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "First Team",
                Color = "123123",
                Season = season,
                SeasonId = season.Id
            };

            _context.Teams.Add(_team);
            _context.SaveChanges();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _teamService = new TeamService(_context);
        }

        [Test]
        public async Task GetTeamByIdSuccess()
        {
            var result = await _teamService!.GetTeamById(_team!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Team team = result.Team!;
            Assert.AreEqual(team.Id, _context!.Teams.First().Id);
            Assert.AreEqual(team.Name, _context!.Teams.First().Name);
            Assert.AreEqual(team.Color, _context!.Teams.First().Color);
        }

        [Test]
        public async Task GetTeamByIdNotFound()
        {
            var result = await _teamService!.GetTeamById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamNotFound"]);
            Assert.IsNull(result.Team);
        }

        [Test]
        public async Task AddTeamSuccess()
        {
            var teamDto = new TeamDto
            {
                Name = "AddTeam1",
                Color = "123123"
            };

            var season = _context!.Seasons.First();

            var result = await _teamService!.AddTeam(season, teamDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Seasons.FirstOrDefaultAsync().Result!.Teams!.Count, 2);

            teamDto.Name = "AddTeam2";
            teamDto.Color = "#123123";
            result = await _teamService!.AddTeam(season, teamDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context.Seasons.FirstOrDefaultAsync().Result!.Teams!.Count, 3);
        }

        [Test]
        public async Task AddTeamMissingName()
        {
            var teamDto = new TeamDto
            {
                Name = "",
                Color = "123123"
            };
            var result = await _teamService!.AddTeam(_context!.Seasons.First(), teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamName"]);
        }

        [Test]
        public async Task AddTeamExists()
        {
            var season = _context!.Seasons.First();

            var result = await _teamService!.AddTeam(season, new TeamDto()
            {
                Name = "First Team",
                Color = "123123"
            });
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamExists"]);
        }

        [Test]
        public async Task AddTeamIncorrectColorCode()
        {
            var teamDto = new TeamDto
            {
                Name = "AddTeam1",
                Color = "WRONGC"
            };

            var season = _context!.Seasons.First();

            var result = await _teamService!.AddTeam(season, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamColor"]);

            teamDto.Color = "#WRONGC";
            result = await _teamService!.AddTeam(season, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamColor"]);

            teamDto.Color = "QWEQWEWRONGC";
            result = await _teamService!.AddTeam(season, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamColor"]);
        }

        [Test]
        public async Task UpdateTeamSuccess()
        {
            var teamDto = new TeamDto
            {
                Name = "New Team",
                Color = "123123"
            };

            var result = await _teamService!.UpdateTeam(_team!, teamDto);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            var findTeam = _context!.Teams.FirstAsync().Result;
            Assert.AreEqual(findTeam.Name, teamDto.Name);
            Assert.AreEqual(findTeam.Color, teamDto.Color);
        }

        [Test]
        public async Task UpdateTeamMissingName()
        {
            var teamDto = new TeamDto
            {
                Name = "",
                Color = "123123"
            };
            var result = await _teamService!.UpdateTeam(_team!, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamName"]);
        }

        [Test]
        public async Task UpdateTeamExists()
        {
            var season = _context!.Seasons.First();

            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Second Team",
                Color = "123123",
                SeasonId = season.Id
            };

            var teamDto = new TeamDto
            {
                Name = "First Team",
                Color = "123123"
            };

            var result = await _teamService!.UpdateTeam(team, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamExists"]);
        }

        [Test]
        public async Task UpdateTeamIncorrectColorCode()
        {
            var teamDto = new TeamDto
            {
                Name = "AddTeam1",
                Color = "WRONGC"
            };
            var result = await _teamService!.UpdateTeam(_team!, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamColor"]);

            teamDto.Color = "#WRONGC";
            result = await _teamService!.UpdateTeam(_team!, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamColor"]);

            teamDto.Color = "QWEQWEWRONGC";
            result = await _teamService!.UpdateTeam(_team!, teamDto);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:TeamColor"]);
        }

        [Test]
        public async Task DeleteTeamSuccess()
        {
            Assert.IsNotEmpty(_context!.Teams);

            var result = await _teamService!.DeleteTeam(_team!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);

            Assert.IsEmpty(_context.Teams);
        }
    }
}
