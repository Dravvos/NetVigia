using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetVigia.DTO;
using NetVigia.BLL.Services.Interfaces;

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
                var sw = Stopwatch.StartNew();
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(website.TimeoutInSeconds));
                var request = new HttpRequestMessage(HttpMethod.Head, website.URL);

                var response = await _httpClient.SendAsync(request, cts.Token);

                sw.Stop();

                result.ResponseTimeInMs = sw.ElapsedMilliseconds;
                result.StatusCode = (int)response.StatusCode;
                result.Up = response.IsSuccessStatusCode && (website.ExpectedStatusCode == 0 || (int)response.StatusCode == website.ExpectedStatusCode);
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
