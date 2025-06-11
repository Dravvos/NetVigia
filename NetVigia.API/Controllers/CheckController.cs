using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command;
using NetVigia.BLL.Query;
using NetVigia.DTO;

namespace NetVigia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        private readonly ISender _sender;

        public CheckController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{url}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetChecks(string url, DateTime startDate, DateTime endDate)
        {
            try
            {
                var cmd = new GetChecksByDate(startDate, endDate, url);
                var checks = await _sender.Send(cmd);
                return Ok(checks);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Check([FromBody] CheckDTO dto)
        {
            try
            {
                var cmd = new AddSiteCheckCommand(dto);
                var @checked = await _sender.Send(cmd);
                if (@checked)
                    return StatusCode(StatusCodes.Status201Created);
                else return StatusCode(StatusCodes.Status500InternalServerError, "Failed to insert check data.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);

                return StatusCode(500, ex.InnerException.Message);
            }
        }
    }
}
