using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace formula1_tournament_api.Controllers
{
    [Route("api/result")]
    [ApiController]
    public class ResultController : Controller
    {
        private Interfaces.IResult _resultService;
        private IUserSeason _userSeasonService;

        public ResultController(Interfaces.IResult resultService, IUserSeason userSeasonService)
        {
            _resultService = resultService;
            _userSeasonService = userSeasonService;
        }

        [HttpGet("{seasonId}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid raceId)
        {
            var result = await _resultService.GetAllResultsByRaceId(raceId);
            if (result.IsSuccess)
            {
                return Ok(result.Results);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{seasonId}/{id}")]
        public async Task<IActionResult> Get(Guid seasonId, Guid id)
        {
            var result = await _resultService.GetResultById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("{seasonId}")]
        public async Task<IActionResult> Post(Guid seasonId, [FromBody] ResultDto resultDto)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _resultService.AddResult(resultDto);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{seasonId}/{id}")]
        public async Task<IActionResult> Put(Guid seasonId, Guid id, [FromBody] ResultDto resultDto)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _resultService.UpdateResult(id, resultDto);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{seasonId}/{id}")]
        public async Task<IActionResult> Delete(Guid seasonId, Guid id)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _resultService.DeleteResult(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
