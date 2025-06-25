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

namespace NetVigia.BLL.Services
{
    public class HttpCheckService : ICheckService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCheckService> _logger;

        public HttpCheckService(HttpClient httpClient, ILogger<HttpCheckService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
                var sw = new Stopwatch();
                if (website.MonitoringType.Sigla == "ICMP")
                {
                    var pingClient = new Ping();
                    sw.Start();
                    var res = await pingClient.SendPingAsync(website.URL.Replace("https://","").Replace("http://",""));
                    sw.Stop();
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
                    
                }
                else
                {
                    using var client = new TcpClient();
                    var connectTask = client.ConnectAsync(website.URL, 80);
                    if(await Task.WhenAny(connectTask, Task.Delay(website.TimeoutInSeconds)) == connectTask)
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
