using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/season")]
    [ApiController]
    public class SeasonController : Controller
    {
        private ISeason _seasonService;
        private IUserSeason _userSeasonService;
        private IUser _userService;
        private ITeam _teamService;

        public SeasonController(ISeason seasonService, IUserSeason userSeasonService, IUser userService, ITeam teamService)
        {
            _seasonService = seasonService;
            _userSeasonService = userSeasonService;
            _userService = userService;
            _teamService = teamService;
        }

        [HttpGet, EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _seasonService.GetSeasons();
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Seasons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _seasonService.GetSeasonById(id);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            
            return Ok(result.Season);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Post([FromBody] SeasonCreateDto seasonDto)
        {
            var resultAdd = await _seasonService.AddSeason(seasonDto, new Guid(User.Identity!.Name!));
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);

            return StatusCode(StatusCodes.Status201Created, resultAdd.SeasonId);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] SeasonUpdateDto seasonDto)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(id);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (!await _userSeasonService.IsAdmin(new Guid(User.Identity!.Name!), id))
                return Forbid();

            var resultUpdate = await _seasonService.UpdateSeason(resultGetSeason.Season!, seasonDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);
            
            return NoContent();
        }

        [HttpPut("{id}/archive"), Authorize]
        public async Task<IActionResult> Archive(Guid id)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(id);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (!await _userSeasonService.IsAdmin(new Guid(User.Identity!.Name!), id))
                return Forbid();

            var resultArchive = await _seasonService.ArchiveSeason(resultGetSeason.Season!);
            if (!resultArchive.IsSuccess)
                return BadRequest(resultArchive.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(id);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (!await _userSeasonService.IsAdmin(new Guid(User.Identity!.Name!), id))
                return Forbid();

            var resultDelete = await _seasonService.DeleteSeason(resultGetSeason.Season!);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);
            
            return NoContent();
        }

        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetByUserId()
        {
            var resultGetSeasons = await _seasonService.GetSeasonsByUserId(new Guid(User.Identity!.Name!));
            if (!resultGetSeasons.IsSuccess)
                return NotFound(resultGetSeasons.ErrorMessage);

            return Ok(resultGetSeasons.Seasons);
        }

        [HttpPost("{seasonId}/user-season"), Authorize]
        public async Task<IActionResult> Post(Guid seasonId, [FromForm] string usernameEmail)
        {
            if (!await _userSeasonService.IsAdmin(new Guid(User.Identity!.Name!), seasonId))
                return Forbid();

            var resultGet = await _userService.GetUserByUsernameEmail(usernameEmail);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            var resultAdd = await _userSeasonService.AddModerator(new Guid(User.Identity!.Name!), resultGet.User!.Id, seasonId);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);

            return NoContent();
        }

        [HttpGet("{seasonId}/driver")]
        public async Task<IActionResult> GetDriversBySeasonId(Guid seasonId)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(seasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            var resultGet = await _seasonService.GetDriversBySeasonId(resultGetSeason.Season!);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            return Ok(resultGet.Drivers);
        }

        [HttpPost("{seasonId}/driver"), Authorize]
        public async Task<IActionResult> PostDriver(Guid seasonId, [FromBody] DriverDto driverDto)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(seasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), seasonId))
                return Forbid();

            Team team = null!;

            if (driverDto.ActualTeamId != null)
            {
                var resultGetTeam = await _teamService.GetTeamById(driverDto.ActualTeamId.GetValueOrDefault());
                if (!resultGetTeam.IsSuccess)
                    return NotFound(resultGetTeam.ErrorMessage);

                team = resultGetTeam.Team!;
            }

            var resultAdd = await _seasonService.AddDriver(resultGetSeason.Season!, driverDto, team);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);
            
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{seasonId}/team")]
        public async Task<IActionResult> GetTeamsBySeasonId(Guid seasonId)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(seasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            var resultGet = await _seasonService.GetTeamsBySeasonId(resultGetSeason.Season!);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            return Ok(resultGet.Teams);
        }

        [HttpPost("{seasonId}/team"), Authorize]
        public async Task<IActionResult> PostTeam(Guid seasonId, [FromBody] TeamDto team)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(seasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), seasonId))
                return Forbid();

            var resultAdd = await _seasonService.AddTeam(resultGetSeason.Season!, team);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);
            
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{seasonId}/race")]
        public async Task<IActionResult> GetRacesBySeasonId(Guid seasonId)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(seasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            var resultGet = await _seasonService.GetRacesBySeasonId(resultGetSeason.Season!);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            return Ok(resultGet.Races);
        }

        [HttpPost("{seasonId}/race"), Authorize]
        public async Task<IActionResult> PostRace(Guid seasonId, [FromBody] RaceDto raceDto)
        {
            var resultGetSeason = await _seasonService.GetSeasonById(seasonId);
            if (!resultGetSeason.IsSuccess)
                return NotFound(resultGetSeason.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), seasonId))
                return Forbid();
            
            var resultAdd = await _seasonService.AddRace(resultGetSeason.Season!, raceDto);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
