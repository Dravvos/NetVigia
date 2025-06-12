using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NetVigia.BLL.Command;
using NetVigia.BLL.Workers.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Workers
{
    public class CheckScheduler : ICheckScheduler, IDisposable
    {

        private readonly ConcurrentDictionary<Guid, (Timer Timer, int Interval)> _timers = new();
        private readonly IMediator _mediator;
        private readonly ILogger<CheckScheduler> _logger;

        public CheckScheduler(IMediator mediator, ILogger<CheckScheduler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public void Dispose()
        {
            foreach (var (timer, _)in _timers.Values)
            {
                timer.Dispose();
            }
            _timers.Clear();
            GC.SuppressFinalize(this);
        }

        public IReadOnlyCollection<Guid> GetScheduledWebsiteIds()
        {
            return _timers.Keys.ToList();
        }

        public void ScheduleWebsiteCheck(ServerDTO website)
        {
            UnscheduleWebsiteCheck(website.Id.GetValueOrDefault());

            var timer = new Timer(async _ =>
            {
                try
                {
                    await _mediator.Send(new AddSiteCheckCommand(website.Id.GetValueOrDefault()));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing scheduled check for website {WebsiteId}", website.Id);
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(website.CheckIntervalSeconds));

            _timers[website.Id.GetValueOrDefault()] = (timer, website.CheckIntervalSeconds);
            _logger.LogDebug("Scheduled checks for website: {WebsiteId} with interval: {Interval}s",
                website.Id, website.CheckIntervalSeconds);
        }

        public void UnscheduleWebsiteCheck(Guid websiteId)
        {
            if (_timers.TryRemove(websiteId, out var timerInfo))
            {
                timerInfo.Timer.Dispose();
                _logger.LogInformation("Unscheduled checks for website {WebsiteId}", websiteId);
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


    }
}
