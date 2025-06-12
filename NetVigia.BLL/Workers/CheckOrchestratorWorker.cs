using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Workers.Interfaces;
using NetVigia.DTO;

namespace NetVigia.Workers
{
    public class CheckOrchestratorWorker : BackgroundService
    {
        
        private readonly ILogger<CheckOrchestratorWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _initializationDelay = TimeSpan.FromSeconds(10);
        private readonly ConcurrentDictionary<Guid, WebsiteSyncState> _syncState = new();


        public CheckOrchestratorWorker(ILogger<CheckOrchestratorWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(_initializationDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested) 
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var serverRepository = scope.ServiceProvider.GetRequiredService<IServerRepository>();
                    var checkScheduler = scope.ServiceProvider.GetRequiredService<ICheckScheduler>();

                    //Carregar todos os sites ativos

                    var activeWebsites = await serverRepository.GetActiveWebsitesAsync();

                    //Sincronizar os agendamentos
                    await SyncSchedules(checkScheduler, activeWebsites, stoppingToken);

                    //Aguardar até a próxima sincronização
                    await Task.Delay(_syncInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CheckOrchestratorWorker: {Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }

        }

        private async Task SyncSchedules(ICheckScheduler scheduler, IEnumerable<ServerDTO> activeWebsites,
        CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var websitesToAddOrUpdate = new List<ServerDTO>();
            var currentScheduledIds = scheduler.GetScheduledWebsiteIds().ToHashSet();

            // 1. Identificar sites novos ou modificados

            foreach (var website in activeWebsites)
            {
                if(!_syncState.TryGetValue(website.Id.GetValueOrDefault(), out var state))
                {
                    //Novo site não registrado
                    websitesToAddOrUpdate.Add(website);
                    _syncState[website.Id.GetValueOrDefault()] = new WebsiteSyncState(website);
                }
                else if(state.HasChanged(website))
                {
                    //Site já registrado, mas com alterações
                    websitesToAddOrUpdate.Add(website);
                    state.Update(website);
                }
            }

            // 2. Identificar sites removidos ou desativados
            var websitesToRemove = new List<Guid>();
            foreach (var siteId in _syncState.Keys)
            {
                var stillActive = activeWebsites.Any(w => w.Id == siteId);
                if (!stillActive)
                {
                    websitesToRemove.Add(siteId);
                }
            }

            // 3. Aplicar mudanças no scheduler
            foreach (var website in websitesToAddOrUpdate)
            {
                try
                {
                    scheduler.ScheduleWebsiteCheck(website);
                    _logger.LogInformation("Scheduled/Updated checks for website: {WebsiteId} ({URL})",
                        website.Id, website.URL);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to schedule website: {WebsiteId}", website.Id);
                }
            }

            foreach (var websiteId in websitesToRemove)
            {
                try
                {
                    scheduler.UnscheduleWebsiteCheck(websiteId);
                    _syncState.TryRemove(websiteId, out _);
                    _logger.LogInformation("Unscheduled checks for removed website: {WebsiteId}", websiteId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to unschedule website: {WebsiteId}", websiteId);
                }
            }

            // 4. Log do estado atual
            _logger.LogInformation("Synchronization completed. Active sites: {ActiveCount}, Scheduled sites: {ScheduledCount}",
                activeWebsites.Count(), scheduler.GetScheduledWebsiteIds().Count);
        }
    }

    internal class WebsiteSyncState
    {
        public Guid WebsiteId { get; }
        public string Url { get; private set; }
        public int CheckInterval { get; private set; }
        public DateTime LastUpdated { get; private set; }

        public WebsiteSyncState(ServerDTO website)
        {
            WebsiteId = website.Id.GetValueOrDefault();
            Update(website);
        }

        public void Update(ServerDTO website)
        {
            Url = website.URL;
            CheckInterval = website.CheckIntervalSeconds;
            LastUpdated = DateTime.UtcNow;
        }

        public bool HasChanged(ServerDTO website)
        {
            return website.URL != Url ||
                   website.CheckIntervalSeconds != CheckInterval;
        }
    }
}
