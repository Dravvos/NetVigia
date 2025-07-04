using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command.Maintenance;
using NetVigia.BLL.Query;
using NetVigia.BLL.Query.Maintenance;
using NetVigia.DTO;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NetVigia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {

        private readonly IMediator _mediator;
        public MaintenanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUser()
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);
                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();
                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;
                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                if (usuarioId == Guid.Empty)
                    return Unauthorized();

                var query = new GetMaintenanceByUserQuery(usuarioId);
                var maintenance = await _mediator.Send(query);
                if (maintenance == null || maintenance.Any() == false)
                    return NotFound();

                return Ok(maintenance);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetByDate(DateTime startDate, DateTime endDate, [FromQuery] List<Guid> ids)
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);
                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();
                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;
                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                if (usuarioId == Guid.Empty)
                    return Unauthorized();

                var ret = new List<MaintenanceDTO>();

                var query = new GetMaintenanceByDateQuery(ids, startDate, endDate);
                ret.AddRange(await _mediator.Send(query));

                return Ok(ret);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetTotalTime/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetTotalMaintenanceDuration(DateTime startDate, DateTime endDate, [FromQuery] List<Guid> serverIds)
        {
            try
            {
                var query = new GetTotalMaintenanceDurationQuery(startDate, endDate, serverIds);
                var time = await _mediator.Send(query);
                return Ok(time);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaintenance([FromBody] MaintenanceDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid maintenance data.");

                dto.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var command = new SaveMaintenanceCommand(dto);
                await _mediator.Send(command);
                return StatusCode(StatusCodes.Status201Created, dto.Id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMaintenance(Guid id, [FromBody] MaintenanceDTO dto)
        {
            try
            {
                if (dto == null || id != dto.Id)
                    return BadRequest("Invalid maintenance data.");

                dto.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var command = new SaveMaintenanceCommand(dto);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMaintenance(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid maintenance ID.");
                var command = new DeleteMaintenanceCommand(id);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
