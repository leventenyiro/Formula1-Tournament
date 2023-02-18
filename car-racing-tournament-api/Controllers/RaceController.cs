﻿using car_racing_tournament_api.DTO;
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

        public RaceController(IRace raceService, IPermission permissionService, IDriver driverService, ITeam teamService, Interfaces.IResult resultService)
        {
            _raceService = raceService;
            _permissionService = permissionService;
            _driverService = driverService;
            _teamService = teamService;
            _resultService = resultService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);
            
            return Ok(resultGet.Race);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] RaceDto raceDto)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Race!.SeasonId))
                return Forbid();
            
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

            var result = await _raceService.DeleteRace(resultGet.Race);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return NoContent();
        }

        [HttpGet("{raceId}/result")]
        public async Task<IActionResult> GetResultsByRaceId(Guid raceId)
        {
            var resultGet = await _raceService.GetRaceById(raceId);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            var result = await _resultService.GetResultsByRace(resultGet.Race!);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            
            return Ok(result.Results);
        }

        [HttpPost("{raceId}/result"), Authorize]
        public async Task<IActionResult> PostResult(Guid raceId, [FromBody] ResultDto resultDto)
        {
            var resultGetRace = await _raceService.GetRaceById(raceId);
            if (!resultGetRace.IsSuccess)
                return NotFound(resultGetRace.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetRace.Race!.SeasonId))
                return Forbid();

            var resultGetDriver = await _driverService.GetDriverById(resultDto.DriverId);
            if (!resultGetDriver.IsSuccess)
                return NotFound(resultGetDriver.ErrorMessage);

            var resultGetTeam = await _teamService.GetTeamById(resultDto.TeamId);
            if (!resultGetTeam.IsSuccess)
                return NotFound(resultGetTeam.ErrorMessage);

            var resultAdd = await _resultService.AddResult(resultGetRace.Race, resultDto, resultGetDriver.Driver!, resultGetTeam.Team!);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);

            /*var resultUpdate = await _driverService.UpdateDriverTeam(resultGetDriver2.Driver!, resultGetTeam2.Team!);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);*/

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
