using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command;
using NetVigia.BLL.Query.Checks;
using NetVigia.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

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

        [HttpGet("{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetChecks(DateTime startDate, DateTime endDate)
        {
            try
            {
                var cmd = new GetChecksByDateQuery(startDate, endDate);
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


        [HttpGet("{serverId:guid}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetChecks(Guid serverId, DateTime startDate, DateTime endDate)
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);
                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var usuarioId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
                if (usuarioId == Guid.Empty)
                    return Unauthorized();

                var cmd = new GetChecksByDateQuery(startDate, endDate, serverId, usuarioId);
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

        [HttpGet("uptime/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetUptimePercentageOfAllServers(DateTime startDate, DateTime endDate)
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);
                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var usuarioId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
                if (usuarioId == Guid.Empty)
                    return Unauthorized();

                var cmd = new GetUptimePercentageOfAllServersQuery(usuarioId, startDate, endDate);
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

        [HttpGet("top5Downtime/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetTop5DowntimeServers(DateTime startDate, DateTime endDate)
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);
                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();
                var usuarioId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
                if (usuarioId == Guid.Empty)
                    return Unauthorized();
             
                var cmd = new GetTop5DowntimeServersQuery(usuarioId, startDate, endDate);
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


        [HttpGet("uptime/{serverId}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetUptimePercentage(Guid serverId, DateTime startDate, DateTime? endDate)
        {
            try
            {
                var cmd = new GetUptimePercentageQuery(serverId, startDate, endDate);
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
