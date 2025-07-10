using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetVigia.BLL.Command;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;
using NetVigia.Work.Workers.Interfaces;

namespace NetVigia.Work.Workers
{
    public class CheckScheduler : ICheckScheduler, IDisposable
    {

        private readonly ConcurrentDictionary<Guid, (Timer Timer, int Interval)> _timers = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CheckScheduler> _logger;
        private bool _disposed;


        public CheckScheduler(IServiceProvider serviceProvider, ILogger<CheckScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            foreach (var (timer, _) in _timers.Values)
            {
                try
                {
                    timer.Dispose();
                }
                catch
                {
                    // Ignore errors during disposal
                }
            }

            _timers.Clear();

            GC.SuppressFinalize(this);
        }

        public IReadOnlyCollection<Guid> GetScheduledWebsiteIds()
        {
            return _timers.Keys.ToList();
        }

        public void ScheduleWebsiteCheck(ServerDTO server)
        {
            UnscheduleWebsiteCheck(server.Id.GetValueOrDefault());

            var timer = new Timer(async _ =>
            {
                await ExecuteCheckAsync(server.Id.GetValueOrDefault());
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(server.CheckIntervalSeconds));

            _timers[server.Id.GetValueOrDefault()] = (timer, server.CheckIntervalSeconds);
            _logger.LogDebug("Scheduled checks for website: {WebsiteId} with interval: {Interval}s",
                server.Id, server.CheckIntervalSeconds);
        }

        public void UnscheduleWebsiteCheck(Guid serverId)
        {
            if (_timers.TryRemove(serverId, out var timerInfo))
            {
                timerInfo.Timer.Dispose();
                _logger.LogInformation("Unscheduled checks for server {ServerId}", serverId);
            }
        }

        private async Task ExecuteCheckAsync(Guid serverId)
        {
            // Criar um escopo manualmente para serviços com escopo
            using var scope = _serviceProvider.CreateScope();
            try
            {
                var serverRepository = scope.ServiceProvider.GetRequiredService<IServerRepository>();
                var maintenanceService = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();

                // Verificar se o site ainda existe
                var server = await serverRepository.GetByIdAsync(serverId);
                if (server == null || !server.Active)
                {
                    UnscheduleWebsiteCheck(serverId);
                    return;
                }

                // Verificar se está em manutenção
                if (await maintenanceService.IsUnderMaintenanceAsync(server))
                {
                    _logger.LogInformation("Skipping check for server {ServerId} due to maintenance", serverId);
                    return;
                }

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new AddSiteCheckCommand(serverId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scheduled check for website {WebsiteId}", serverId);
            }
        }

        public void UpdateWebsiteCheckSchedule(ServerDTO website)
        {
            if (_timers.TryGetValue(website.Id.GetValueOrDefault(), out var existing))
            {
                // Atualizar apenas se o intervalo mudou
                if (existing.Interval != website.CheckIntervalSeconds)
                {
                    _logger.LogInformation("Updating interval for website {WebsiteId}: {Old}s → {New}s",
                        website.Id, existing.Interval, website.CheckIntervalSeconds);

                    ScheduleWebsiteCheck(website);
                }
            }
            else
            {
                // Se não existir, adicionar novo
                ScheduleWebsiteCheck(website);
            }
        }

        public bool IsScheduled(Guid websiteId)
        {
            return _timers.TryGetValue(websiteId, out _);
        }
    }
}
