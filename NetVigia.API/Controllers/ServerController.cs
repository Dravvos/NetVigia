using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command;
using NetVigia.BLL.Command.Server;
using NetVigia.BLL.Query;
using NetVigia.DTO;

namespace NetVigia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerController : ControllerBase
    {
        private readonly ISender _sender;

        public ServerController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetServersByUser()
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
                    return UnprocessableEntity("Id do usuário não pode ser vazio");

                var cmd = new GetServersByUserQuery(usuarioId);
                var servers = await _sender.Send(cmd);
                if (servers == null || !servers.Any())
                    return NotFound("Nenhum servidor encontrado para o usuário");

                return Ok(servers);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddServer([FromBody] ServerDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados do servidor não podem ser nulos");

                var cmd = new SaveServerCommand(dto);
                await _sender.Send(cmd);

                return StatusCode(StatusCodes.Status201Created, "Servidor adicionado com sucesso");
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServer(Guid id, [FromBody] ServerDTO dto)
        {
            try
            {
                if (dto == null || dto.Id != id)
                    return BadRequest("Dados do servidor inválidos");

                var cmd = new SaveServerCommand(dto);
                await _sender.Send(cmd);
                return NoContent();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Id do servidor não pode ser vazio");

                var cmd = new DeleteServerCommand(id);
                if(await _sender.Send(cmd))
                    return NoContent();
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir o servidor. Verifique se ele está vinculado a outros dados.");
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