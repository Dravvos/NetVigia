using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command;
using NetVigia.BLL.Query;
using NetVigia.DTO;

namespace NetVigia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckController : ControllerBase
    {
        private readonly ISender _sender;

        public CheckController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{serverId:guid}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetChecks(Guid serverId, DateTime startDate, DateTime endDate, [FromQuery] int count = 20)
        {
            try
            {
                var cmd = new GetChecksByDateQuery(startDate, endDate, serverId);
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

        [HttpGet("uptime/{serverId}")]
        public async Task<IActionResult> GetUptimePercentage(Guid serverId, [FromQuery] int hours = 24)
        {
            try
            {
                var period = TimeSpan.FromHours(hours);
                var cmd = new GetUptimePercentageQuery(serverId, period);
                var uptimePercentage = await _sender.Send(cmd);
                return Ok(uptimePercentage);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpGet("responseTime/{serverId}")]
        public async Task<IActionResult> GetResponseTime(Guid serverId, [FromQuery] int hours = 24)
        {
            try
            {
                var period = TimeSpan.FromHours(hours);
                var cmd = new GetAverageResponseTimeQuery(serverId, period);
                var responseTime = await _sender.Send(cmd);
                return Ok(responseTime);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpGet("responseTimeByDate/{serverId}")]
        public async Task<IActionResult> GetResponseTimeByDate(Guid serverId, [FromQuery] int hours = 24)
        {
            try
            {
                var period = TimeSpan.FromHours(hours);
                var cmd = new GetAverageResponseTimeByDateQuery(serverId,period);
                var chartInfo = await _sender.Send(cmd);
                return Ok(chartInfo);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost("perform/{serverId:guid}")]
        public async Task<IActionResult> PerformCheck(Guid serverId)
        {
            try
            {
                var cmd = new AddSiteCheckCommand(serverId);
                var check = await _sender.Send(cmd);
                if (check == null)
                    return NotFound("Check could not be performed.");
                return Ok(check);
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
