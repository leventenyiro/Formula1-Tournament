﻿using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/driver")]
    [ApiController]
    public class DriverController : Controller
    {
        private IDriver _driverService;
        private IPermission _permissionService;
        private ITeam _teamService;
        private ISeason _seasonService;
        private IConfiguration _configuration;

        public DriverController(
            IDriver driverService, 
            IPermission permissionService, 
            ITeam teamService,
            ISeason seasonService)
        {
            _driverService = driverService;
            _permissionService = permissionService;
            _teamService = teamService;
            _seasonService = seasonService;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] DriverDto driverDto)
        {
            var resultGetDriver = await _driverService.GetDriverById(id);
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

            Team team = null!;

            if (driverDto.ActualTeamId != null)
            {
                var resultGetTeam = await _teamService.GetTeamById(driverDto.ActualTeamId.GetValueOrDefault());
                if (!resultGetTeam.IsSuccess)
                    return NotFound(resultGetTeam.ErrorMessage);

                team = resultGetTeam.Team!;
            }

            var resultUpdate = await _driverService.UpdateDriver(resultGetDriver.Driver, driverDto, team!);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _driverService.GetDriverById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Driver!.SeasonId))
                return Forbid();

            var resultGetSeason = await _seasonService.GetSeasonById(resultGet.Driver.SeasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (resultGetSeason.Season!.IsArchived) {
                return BadRequest(_configuration["ErrorMessages:SeasonArchived"]);
            }

            var resultDelete = await _driverService.DeleteDriver(resultGet.Driver);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }

        [HttpGet("statistics/{name}")]
        public async Task<IActionResult> Statistics(string name) {
            var result = await _driverService.GetStatistics(name);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.DriverStatistics);
        }

        [HttpGet("nationality")]
        public async Task<IActionResult> Nationality() {
            var result = await _driverService.Nationalities();
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Nationalities);
        }
    }
}
