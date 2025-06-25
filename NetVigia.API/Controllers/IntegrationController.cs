using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command.Integration;
using NetVigia.BLL.Command.Server;
using NetVigia.BLL.Query;
using NetVigia.DTO;

namespace NetVigia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntegrationController : ControllerBase
    {
        private readonly ISender _sender;

        public IntegrationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetIntegrationsByUser()
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
                var query = new GetIntegrationsByUserQuery(usuarioId);
                var integrations = await _sender.Send(query);
                return Ok(integrations);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateIntegration([FromBody] IntegrationDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados da integração não podem ser nulos");
                dto.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var cmd = new SaveIntegrationCommand(dto);
                await _sender.Send(cmd);

                return StatusCode(StatusCodes.Status201Created, "Integração criada com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateIntegration(Guid id, [FromBody] IntegrationDTO dto)
        {
            try
            {
                if (dto == null || id == Guid.Empty)
                    return BadRequest("Dados da integração não podem ser nulos");

                dto.Id = id;
                dto.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var cmd = new SaveIntegrationCommand(dto);
                await _sender.Send(cmd);
                return Ok("Integração atualizada com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteIntegration(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID da integração não pode ser nulo");
                var cmd = new DeleteIntegrationCommand(id);
                await _sender.Send(cmd);
                return Ok("Integração excluída com sucesso");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
            }
        }
    }
}