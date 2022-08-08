using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace formula1_tournament_api.Controllers
{
    [Route("api/racer")]
    [ApiController]
    public class RacerController : Controller
    {
        private IRacer _racerService;

        public RacerController(IRacer racerService)
        {
            _racerService = racerService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _racerService.GetAllRacers();
            if (result.IsSuccess)
            {
                return Ok(result.Racer);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _racerService.GetRacerById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Racer);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Racer racer)
        {
            if ()
            var result = await _racerService.AddRacer(racer);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Racer racer)
        {
            var result = await _racerService.UpdateRacer(id, racer);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _racerService.DeleteRacer(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
