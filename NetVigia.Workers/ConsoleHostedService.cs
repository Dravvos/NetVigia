using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetVigia.Workers.Workers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Workers
{
    public class ConsoleHostedService : BackgroundService
    {
        private readonly ILogger<ConsoleHostedService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ICheckScheduler _checkScheduler;

        public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime hostApplicationLifetime, ICheckScheduler checkScheduler)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _checkScheduler = checkScheduler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Aplicação iniciada. Pressione Ctrl+C para sair.");

            var tcs = new TaskCompletionSource<bool>();

            using var reg = stoppingToken.Register(() =>
            {
                _logger.LogInformation("Iniciando desligamento gracioso...");
                tcs.TrySetResult(true);
            });

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                tcs.TrySetResult(true);
            };

            await tcs.Task;

            try
            {
                // Liberar recursos antes de desligar
                if (_checkScheduler is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _logger.LogInformation("Monitoramento desligado com sucesso.");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
