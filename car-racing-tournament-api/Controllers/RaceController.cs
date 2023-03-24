using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/race")]
    [ApiController]
    public class RaceController : Controller
    {
        private IRace _raceService;
        private IPermission _permissionService;
        private IDriver _driverService;
        private ITeam _teamService;
        private Interfaces.IResult _resultService;
        private ISeason _seasonService;
        private IConfiguration _configuration;

        public RaceController(
            IRace raceService, 
            IPermission permissionService, 
            IDriver driverService, 
            ITeam teamService, 
            Interfaces.IResult resultService,
            ISeason seasonService,
            IConfiguration configuration)
        {
            _raceService = raceService;
            _permissionService = permissionService;
            _driverService = driverService;
            _teamService = teamService;
            _resultService = resultService;
            _seasonService = seasonService;
            _configuration = configuration;
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] RaceDto raceDto)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Race!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGet.Race.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }
            
            var resultUpdate = await _raceService.UpdateRace(resultGet.Race, raceDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);
            
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Race!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGet.Race.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var result = await _raceService.DeleteRace(resultGet.Race);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return NoContent();
        }

        [HttpPost("{raceId}/result"), Authorize]
        public async Task<IActionResult> PostResult(Guid raceId, [FromBody] ResultDto resultDto)
        {
            var resultGetRace = await _raceService.GetRaceById(raceId);
            if (!resultGetRace.IsSuccess)
                return NotFound(resultGetRace.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetRace.Race!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGetRace.Race.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var resultGetDriver = await _driverService.GetDriverById(resultDto.DriverId);
            if (!resultGetDriver.IsSuccess)
                return NotFound(resultGetDriver.ErrorMessage);    

            var resultGetTeam = await _teamService.GetTeamById(resultDto.TeamId);
            if (!resultGetTeam.IsSuccess)
                return NotFound(resultGetTeam.ErrorMessage);     

            var resultAdd = await _resultService.AddResult(resultGetRace.Race!, resultDto, resultGetDriver.Driver!, resultGetTeam.Team!);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
