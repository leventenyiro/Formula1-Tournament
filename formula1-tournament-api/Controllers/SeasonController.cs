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

        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetByUserId()
        {
            var result1 = await _userSeasonService.GetAllOwnedSeasonId(new Guid(User.Identity.Name));
            if (!result1.IsSuccess)
                return NotFound(result1.ErrorMessage);
            var result2 = await _seasonService.GetAllSeasonsByUserSeasonList(result1.UserSeasons.Select(x => x.SeasonId).ToList());
            if (!result2.IsSuccess)
                return NotFound(result2.ErrorMessage);
            return Ok(result2.Seasons);
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
        public async Task<IActionResult> Post([FromBody] string name)
        {
            var result = await _seasonService.AddSeason(name, new Guid(User.Identity.Name));
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromForm] string name)
        {
            var result = await _seasonService.UpdateSeason(id, name);
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
