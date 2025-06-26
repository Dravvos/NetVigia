using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command;
using NetVigia.BLL.Query.Checks;
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
        public async Task<IActionResult> GetChecks(Guid serverId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var cmd = new GetChecksByDateQuery(startDate, endDate, serverId);
                var checks = await _sender.Send(cmd);
                if (checks == null || checks.Any() == false)
                    return NotFound();

                return Ok(checks);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpGet("failed/{serverId:guid}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetFailedChecks(Guid serverId, DateTime startDate, DateTime endDate, [FromQuery] int count = 20)
        {
            try
            {
                var cmd = new GetFailedChecksQuery(serverId, startDate, endDate);
                var checks = await _sender.Send(cmd);
                if (checks == null || checks.Any() == false)
                    return NotFound();

                return Ok(checks);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpGet("failedByDate/{serverId:guid}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetFailedChecksByDate(Guid serverId, DateTime startDate, DateTime endDate, [FromQuery] int count = 20)
        {
            try
            {
                var cmd = new GetFailedChecksByDateQuery(serverId, startDate, endDate);
                var checks = await _sender.Send(cmd);
                if (checks == null || checks.Any() == false)
                    return NotFound();

                return Ok(checks);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }


        [HttpGet("uptime/{serverId}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetUptimePercentage(Guid serverId, DateTime startDate, DateTime? endDate)
        {
            try
            {
                var cmd = new GetUptimePercentageQuery(serverId, startDate,endDate);
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

        [HttpGet("responseTime/{serverId:guid}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetResponseTime(Guid serverId, DateTime startDate, DateTime? endDate)
        {
            try
            {
                var cmd = new GetAverageResponseTimeQuery(serverId, startDate, endDate);
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

        [HttpGet("responseTimeByDate/{serverId:guid}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetResponseTimeByDate(Guid serverId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var cmd = new GetAverageResponseTimeByDateQuery(serverId, startDate, endDate);
                var chartInfo = await _sender.Send(cmd);
                if (chartInfo == null || chartInfo.Any() == false)
                    return NotFound();

                return Ok(chartInfo);
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
