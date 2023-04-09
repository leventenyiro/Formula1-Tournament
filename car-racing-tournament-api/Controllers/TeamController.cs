using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/team")]
    [ApiController]
    public class TeamController : Controller
    {
        private ITeam _teamService;
        private IPermission _permissionService;
        private ISeason _seasonService;
        private IConfiguration _configuration;
        private IDriver _driverService;
        private Interfaces.IResult _resultService;

        public TeamController(
            ITeam teamService, 
            IPermission permissionService,
            ISeason seasonService,
            IDriver driverService,
            Interfaces.IResult resultService)
        {
            _teamService = teamService;
            _permissionService = permissionService;
            _seasonService = seasonService;
            _driverService = driverService;
            _resultService = resultService;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] TeamDto teamDto)
        {
            var resultGet = await _teamService.GetTeamById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Team!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGet.Team.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var resultUpdate = await _teamService.UpdateTeam(resultGet.Team, teamDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _teamService.GetTeamById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Team!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGet.Team.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var resultDriverNullActualTeam = await _driverService.SetActualTeamNullByTeamId(resultGet.Team.Id);
            if (!resultDriverNullActualTeam.IsSuccess)
                return NotFound(resultDriverNullActualTeam.ErrorMessage);

            var resultDeleteResultsByTeam = await _resultService.DeleteResultsByTeamId(resultGet.Team.Id);
            if (!resultDeleteResultsByTeam.IsSuccess)
                return NotFound(resultDeleteResultsByTeam.ErrorMessage);

            var resultDelete = await _teamService.DeleteTeam(resultGet.Team);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
