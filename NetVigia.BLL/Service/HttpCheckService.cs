using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetVigia.DTO;
using NetVigia.BLL.Services.Interfaces;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.Services
{
    public class HttpCheckService : ICheckService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCheckService> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IMaintenanceService _maintenanceService;

        public HttpCheckService(HttpClient httpClient, ILogger<HttpCheckService> logger, IIntegrationService integrationService, IMaintenanceService maintenanceService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _integrationService = integrationService;
            _maintenanceService = maintenanceService;
        }

        public async Task<CheckDTO> PerformCheckAsync(ServerDTO website)
        {
            var result = new CheckDTO
            {
                ServerId = website.Id.GetValueOrDefault(),
                Timestamp = DateTime.UtcNow
            };
            try
            {

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(website.TimeoutInSeconds));

                if (await _maintenanceService.IsUnderMaintenanceAsync(website))
                {
                    _logger.LogInformation("Website {Url} is under maintenance, skipping check", website.URL);
                    result.Up = true;
                    result.StatusCode = 0;
                    result.ResponseTimeInMs = 0;
                    result.ErrorMessage = "Sob manutenção";
                    result.Timestamp = DateTime.UtcNow;
                    result.Server = website;

                    return result;
                }

                var sw = new Stopwatch();
                if (website.MonitoringType.Sigla == "ICMP")
                {
                    var pingClient = new Ping();
                    var res = await pingClient.SendPingAsync(website.URL.Replace("https://", "").Replace("http://", ""));
                    result.ResponseTimeInMs = res.RoundtripTime;
                    switch (res.Status)
                    {
                        case IPStatus.Unknown:
                            result.StatusCode = 500;
                            result.Up = false;
                            break;
                        case IPStatus.Success:
                            result.StatusCode = 200;
                            result.Up = true;
                            break;
                        case IPStatus.DestinationNetworkUnreachable:
                            result.StatusCode = 523;
                            result.Up = false;
                            break;
                        case IPStatus.DestinationHostUnreachable:
                            result.StatusCode = 523;
                            result.Up = false;
                            break;
                        case IPStatus.DestinationProhibited:
                            result.StatusCode = 403;
                            result.Up = true;
                            break;
                        case IPStatus.DestinationPortUnreachable:
                            result.StatusCode = 523;
                            result.Up = false;
                            break;
                        case IPStatus.TimedOut:
                            result.StatusCode = 522;
                            result.Up = false;
                            break;
                        case IPStatus.BadDestination:
                            result.StatusCode = 400;
                            result.Up = false;
                            break;
                        case IPStatus.DestinationUnreachable:
                            result.StatusCode = 523;
                            result.Up = false;
                            break;
                        default:
                            result.StatusCode = 500;
                            result.Up = false;
                            break;
                    }
                }
                else if (website.MonitoringType.Sigla == "HTTP")
                {
                    var httpMethod = website.HTTPMethod.Sigla;
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(website.URL);

                    switch (httpMethod)
                    {
                        case "GET":
                            request.Method = HttpMethod.Get;
                            break;
                        case "POST":
                            request.Method = HttpMethod.Post;
                            break;
                        case "PUT":
                            request.Method = HttpMethod.Put;
                            break;
                        case "DELETE":
                            request.Method = HttpMethod.Delete;
                            break;
                        case "HEAD":
                            request.Method = HttpMethod.Head;
                            break;
                        case "PATCH":
                            request.Method = HttpMethod.Patch;
                            break;
                        case "OPTIONS":
                            request.Method = HttpMethod.Options;
                            break;
                        default:
                            request.Method = HttpMethod.Head;
                            break;
                    }
                    sw.Start();
                    var response = await _httpClient.SendAsync(request, cts.Token);
                    sw.Stop();

                    result.StatusCode = (int)response.StatusCode;
                    result.Up = response.IsSuccessStatusCode && (website.ExpectedStatusCode == 0 || (int)response.StatusCode == website.ExpectedStatusCode);

                    var integrations = await _integrationService.GetByUserAsync(website.UserId);
                    if (integrations != null && integrations.Any())
                    {
                        foreach (var integration in integrations)
                        {
                            if (integration.IntegrationMethod.Sigla != "TEL") //Telegram
                            {
                                try
                                {
                                    object obj = new();

                                    if (result.Up)
                                    {
                                        obj = new
                                        {
                                            @event = "check_up",
                                            check = new
                                            {
                                                websiteId = website.Id,
                                                url = website.URL,
                                                statusCode = result.StatusCode,
                                                down = false,
                                                date = DateTime.UtcNow.Ticks,
                                            }
                                        };
                                    }
                                    else
                                    {
                                        obj = new
                                        {
                                            @event = "check_down",
                                            check = new
                                            {
                                                websiteId = website.Id,
                                                url = website.URL,
                                                statusCode = result.StatusCode,
                                                down = true,
                                                date = DateTime.UtcNow.Ticks,
                                            },
                                            request = new
                                            {
                                                method = request.Method.Method,
                                                url = request.RequestUri.ToString(),
                                                headers = request.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                                                body = request.Content != null ? await request.Content.ReadAsStringAsync(cts.Token) : null
                                            },
                                            response = new
                                            {
                                                statusCode = result.StatusCode,
                                                headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                                                body = await response.Content.ReadAsStringAsync(cts.Token),
                                                responseTimeInMs = result.ResponseTimeInMs
                                            }
                                        };
                                    }

                                    await _integrationService.SendNotificationAsync(integration.IntegrationEndpoint, obj);

                                    _logger.LogInformation("Notification for integration {IntegrationId} was sent successfully", integration.Id);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error sending notification for integration {IntegrationId}", integration.Id);
                                }
                            }
                            else
                            {
                                try
                                {
                                    var message = result.Up
                                        ? $"Check UP: {website.URL} - Status Code: {result.StatusCode}"
                                        : $"Check DOWN: {website.URL} - Status Code: {result.StatusCode}";

                                    await _integrationService.SendNotificationAsync(integration.IntegrationEndpoint, message);

                                    _logger.LogInformation("Telegram notification for integration {IntegrationId} was sent successfully", integration.Id);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error sending Telegram notification for integration {IntegrationId}", integration.Id);
                                }
                            }
                        }
                    }
                }
                else
                {
                    using var client = new TcpClient();
                    var connectTask = client.ConnectAsync(website.URL, 80);
                    if (await Task.WhenAny(connectTask, Task.Delay(website.TimeoutInSeconds)) == connectTask)
                    {
                        result.Up = client.Connected;
                        result.StatusCode = 200;
                    }
                    else
                    {
                        result.Up = false;
                        result.StatusCode = 408;
                    }

                }


                if (website.MonitoringType.Sigla != "ICMP")
                    result.ResponseTimeInMs = sw.ElapsedMilliseconds;

                if (!string.IsNullOrEmpty(website.ExpectedContent) && result.Up)
                {
                    // Se precisar verificar conteúdo, fazemos uma requisição GET
                    var contentResponse = await _httpClient.GetAsync(website.URL, cts.Token);
                    var content = await contentResponse.Content.ReadAsStringAsync(cts.Token);

                    result.Up = content.Contains(website.ExpectedContent);
                    if (!result.Up)
                    {
                        result.ErrorMessage = "Expected content not found";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking website {Url}", website.URL);
                result.Up = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
