using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
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
                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;
                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
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

        [HttpGet("certificate/{url}")]
        public async Task<IActionResult> GetCertificateInfo(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return BadRequest("URL não pode ser vazia");
                
                url = Uri.UnescapeDataString(url); // Decodifica a URL para lidar com caracteres especiais
                var cert = CheckCertificate(url);
                if (cert == null)
                    return NotFound();

                return Ok(cert);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);

            }
        }


        private object? CheckCertificate(string url)
        {
            object? ret = null;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    if (cert == null)
                    {
                        return false;
                    }

                    ret = new
                    {
                        Dominio = cert.Subject,
                        Emissor = cert.Issuer,
                        ValidoAte = cert.GetExpirationDateString(),
                        Errors = errors,
                        IsValid = IsCertificateValid(new X509Certificate2(cert)),
                        DiasRestantes = (cert.NotAfter - DateTime.UtcNow).Days,
                    };
                    // Exibe informações do certificado
                    PrintCertificateInfo(new X509Certificate2(cert));

                    // Retorna `true` para ignorar erros de validação (ex: expirado)
                    // Ou `false` para falhar a requisição em caso de problemas
                    return true;
                }
            };
            using (var client = new HttpClient(handler))
            {
                var response = client.GetAsync(url).Result;
                Console.WriteLine($"Status da requisição: {response.StatusCode}");
            }
            return ret;

        }

        private void PrintCertificateInfo(X509Certificate2 cert)
        {
            Console.WriteLine("=== SSL Certificate Info ===");
            Console.WriteLine($"Domínio: {cert.Subject}");
            Console.WriteLine($"Emissor: {cert.Issuer}");
            Console.WriteLine($"Válido até: {cert.GetExpirationDateString()}");
            Console.WriteLine($"Dias restantes: {(cert.NotAfter - DateTime.UtcNow).Days}");
            Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
            Console.WriteLine($"É válido: {IsCertificateValid(cert)}");
            Console.WriteLine("============================");
        }

        private bool IsCertificateValid(X509Certificate2 cert)
        {
            return DateTime.Now < cert.NotAfter && DateTime.Now > cert.NotBefore;
        }

    }
}
