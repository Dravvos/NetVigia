using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetVigia.API.Utils;
using NetVigia.BLL.Command.TabelaGeral;
using NetVigia.BLL.Query.TabelaGeral;
using NetVigia.DTO;

namespace NetVigia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TabelaGeralController : ControllerBase
    {
        private readonly ISender _sender;

        public TabelaGeralController(ISender sender)
        {
            _sender = sender;
        }

        [Route("[action]")]
        [HttpPost, Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AddTabelaGeral([FromBody] TabelaGeralDTO dto)
        {
            try
            {
                dto.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var cmd = new SaveTabelaGeralCommand(dto);
                await _sender.Send(cmd);
                return StatusCode(StatusCodes.Status201Created, dto.Id);

            }
            catch (ArgumentException ex) { return BadRequest(ex); }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                else
                    return StatusCode(500, ex.InnerException.Message);
            }
        }

        [Route("[action]/{tabelaGeralId}")]
        [HttpPut, Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateTabelaGeral(Guid tabelaGeralId, [FromBody] TabelaGeralDTO dto)
        {
            if (tabelaGeralId != dto.Id) return BadRequest("Id da requisição é diferente do Id do Body");
            try
            {
                dto.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var cmd = new SaveTabelaGeralCommand(dto);
                await _sender.Send(cmd);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                if (ex.InnerException == null)
                    return BadRequest(ex.Message);

                return BadRequest(ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                else
                    return StatusCode(500, ex.InnerException.Message);
            }

        }

        [Route("[action]/{nome}")]
        [HttpGet, Authorize]
        public async Task<IActionResult> GetTabelaGeralByNome(string nome)
        {
            try
            {
                var cmd = new GetTabelaGeralByNomeQuery(nome);
                var tabelaGeral = await _sender.Send(cmd);
                if (tabelaGeral == null)
                    return NotFound();
                return Ok(tabelaGeral);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }

        [Route("[action]/{id}")]
        [HttpGet, Authorize]
        public async Task<IActionResult> GetTabelaGeralItemById(Guid id)
        {
            var cmd = new GetTabelaGeralItemByIdQuery(id);
            var tabelaGeral = await _sender.Send(cmd);
            if (tabelaGeral == null)
                return NotFound();

            return Ok(tabelaGeral);
        }

        [Route("[action]/{tabelaGeralId:guid}/{sigla}")]
        [HttpGet, Authorize]
        public async Task<IActionResult> GetTabelaGeralItem(Guid tabelaGeralId, string sigla)
        {
            try
            {
                var cmd = new GetTabelaGeralItemQuery(tabelaGeralId, sigla);
                var tabelaGeral = await _sender.Send(cmd);
                if (tabelaGeral == null)
                    return NotFound();

                return Ok(tabelaGeral);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                else
                    return StatusCode(500, ex.InnerException.Message);
            }
        }

        [Route("[action]")]
        [HttpGet, Authorize]
        public async Task<IActionResult> GetTabelasGeraisItems(Guid? tabelaGeralId = null)
        {
            try
            {

                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var query = new GetTabelaGeralItensByTabelaGeralIdQuery(tabelaGeralId);
                var tabelasGerais = await _sender.Send(query);

                if (tabelasGerais == null || tabelasGerais.Any() == false)
                    return NotFound();

                return Ok(tabelasGerais);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost, Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AddTabelaGeralItem([FromBody] TabelaGeralItemDTO dto)
        {
            try
            {
                dto.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var cmd = new SaveTabelaGeralItemCommand(dto);
                await _sender.Send(cmd);

                return StatusCode(StatusCodes.Status201Created, dto.Id);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                else
                    return StatusCode(500, ex.InnerException.Message);
            }

        }

        [Route("[action]/{tabelaGeralItemId}")]
        [HttpPut, Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateTabelaGeralItem(Guid tabelaGeralItemId, [FromBody] TabelaGeralItemDTO dto)
        {
            if (tabelaGeralItemId != dto.Id) return BadRequest();
            try
            {
                dto.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                var cmd = new SaveTabelaGeralItemCommand(dto);
                await _sender.Send(cmd);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                if (ex.InnerException == null)
                    return BadRequest(ex.Message);

                return BadRequest(ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                else
                    return StatusCode(500, ex.InnerException.Message);
            }
        }

        [Route("[action]/{tabelaGeralItemId}")]
        [HttpDelete, Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteTabelaGeralItem(Guid tabelaGeralItemId)
        {
            try
            {
                var cmd = new DeleteTabelaGeralItemCommand(tabelaGeralItemId);
                await _sender.Send(cmd);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                else
                    return StatusCode(500, ex.InnerException.Message);
            }
        }
    }
}
