using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/result")]
    [ApiController]
    public class ResultController : Controller
    {
        private Interfaces.IResult _resultService;
        private IPermission _permissionService;
        private IDriver _driverService;
        private ITeam _teamService;
        private IRace _raceService;
        private ISeason _seasonService;
        private IConfiguration _configuration;

        public ResultController(
            Interfaces.IResult resultService, 
            IPermission permissionService, 
            IDriver driverService, 
            ITeam teamService, 
            IRace raceService,
            ISeason seasonService)
        {
            _resultService = resultService;
            _permissionService = permissionService;
            _driverService = driverService;
            _teamService = teamService;
            _raceService = raceService;
            _seasonService = seasonService;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> PostResult([FromBody] ResultDto resultDto)
        {
            var resultGetRace = await _raceService.GetRaceById(resultDto.RaceId);
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

            return StatusCode(StatusCodes.Status201Created, resultAdd.Id);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] ResultDto resultDto)
        {
            var resultGetResult = await _resultService.GetResultById(id);
            if (!resultGetResult.IsSuccess)
                return NotFound(resultGetResult.ErrorMessage);

            var resultGetDriver = await _driverService.GetDriverById(resultGetResult.Result!.Driver.Id);
            if (!resultGetDriver.IsSuccess)
                return NotFound(resultGetDriver.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetDriver.Driver!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGetDriver.Driver.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var resultGetTeam = await _teamService.GetTeamById(resultGetResult.Result!.TeamId);
            if (!resultGetTeam.IsSuccess)
                return NotFound(resultGetTeam.ErrorMessage);

            var resultGetRace = await _raceService.GetRaceById(resultGetResult.Result!.RaceId);
            if (!resultGetRace.IsSuccess)
                return NotFound(resultGetRace.ErrorMessage);

            var resultUpdate = await _resultService.UpdateResult(resultGetResult.Result, resultDto, resultGetRace.Race!, resultGetDriver.Driver, resultGetTeam.Team!);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGetResult = await _resultService.GetResultById(id);
            if (!resultGetResult.IsSuccess)
                return NotFound(resultGetResult.ErrorMessage);

            var resultGetDriver = await _driverService.GetDriverById(resultGetResult.Result!.Driver.Id);
            if (!resultGetDriver.IsSuccess)
                return BadRequest(resultGetDriver.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetDriver.Driver!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGetDriver.Driver.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var resultDelete = await _resultService.DeleteResult(resultGetResult.Result);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
