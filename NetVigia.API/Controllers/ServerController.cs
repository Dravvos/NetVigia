using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.BLL.Command;
using NetVigia.BLL.Command.Server;
using NetVigia.BLL.Query;
using NetVigia.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var query = new GetServersByIdQuery(id);
                var server = await _sender.Send(query);
                if (server == null)
                    return NotFound();
                if (server.UserId != Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)))
                    return Forbid("You do not have permission to access this server");
                return Ok(server);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServersByUser()
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var usuarioId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
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
                dto.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
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
                dto.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
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

                var server = await _sender.Send(new GetServersByIdQuery(id));
                if (server.UserId != Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)))
                    return Forbid("You do not have permission to delete this server");
                var cmd = new DeleteServerCommand(id);
                await _sender.Send(cmd);
                return NoContent();
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