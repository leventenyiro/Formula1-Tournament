using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace formula1_tournament_api.Controllers
{
    [Route("api/season")]
    [ApiController]
    public class SeasonController : Controller
    {
        private ISeason _seasonService;
        private IUserSeason _userSeasonService;

        public SeasonController(ISeason seasonService, IUserSeason userSeasonService)
        {
            _seasonService = seasonService;
            _userSeasonService = userSeasonService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _seasonService.GetAllSeasons();
            if (result.IsSuccess)
            {
                return Ok(result.Seasons);
            }
            return NotFound(result.ErrorMessage);
        }

        // should be tested
        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result1 = await _userSeasonService.GetAllOwnedSeasonId(userId);
            if (!result1.IsSuccess)
                return NotFound(result1.ErrorMessage);
            var result2 = await _seasonService.GetAllSeasons();
            if (!result2.IsSuccess)
                return NotFound(result2.ErrorMessage);
            var result = result2.Seasons.Join(
                result1.UserSeasons,
                season => season.Id,
                userSeason => userSeason.SeasonId,
                (season, userSeason) => new { Season = season, UserSeason = userSeason });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _seasonService.GetSeasonById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Season);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Post([FromBody] SeasonDto season)
        {
            var result = await _seasonService.AddSeason(season, new Guid(User.Identity.Name));
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] SeasonDto season)
        {
            var result = await _seasonService.UpdateSeason(id, season);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _seasonService.DeleteSeason(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
