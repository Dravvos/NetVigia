using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.IoTDB;
using Microsoft.Extensions.Logging;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Service
{
    public class IoTDBService : IIoTDBService
    {
        private readonly IIoTDBRepository _repository;
        private readonly IServerRepository _serverRepository;
        private readonly ILogger<IoTDBService> _logger;

        public IoTDBService(IIoTDBRepository repository, IServerRepository serverRepository, ILogger<IoTDBService> logger)
        {
            _repository = repository;
            _serverRepository = serverRepository;
            _logger = logger;
        }

        private string SanitizeSiteUrl(string url) =>
      url.Replace("https://", "")
         .Replace("http://", "")
         .Replace("/", "_")
         .Replace(".", "_")
         .Replace(":", "_");

        public async Task<bool> Insert(CheckDTO check)
        {
            _logger.LogInformation("Inserting check for server {ServerId} at {Timestamp}", check.Server.Id, check.Timestamp);

            string url = $"root.netvigia.{SanitizeSiteUrl(check.Server.URL)}";

            var measurements = new List<string>
            {
                "StatusCode", "Latency", "Up"
            };
            var dataTypes = new List<TSDataType>
            {
                TSDataType.INT32, TSDataType.FLOAT, TSDataType.BOOLEAN
            };
            var values = new List<object>
            {
                check.StatusCode,
                check.ResponseTimeInMs,
                check.Up
            };
            bool success = await _repository.InsertCheck(values, measurements, check.Timestamp, dataTypes, url);
            if (!success)
            {
                _logger.LogError("Failed to insert check for server {ServerId} at {Timestamp}", check.Server.Id, check.Timestamp);
            }
            else
            {
                _logger.LogInformation("Successfully inserted check for server {ServerId} at {Timestamp}", check.Server.Id, check.Timestamp);
            }
            return success;
        }

        public async Task<List<CheckDTO>> ListChecks(Guid serverId, DateTime startDate, DateTime? endDate)
        {

            var server = await _serverRepository.GetByIdAsync(serverId);
            if (server == null)
                return new List<CheckDTO>();

            string url = server.URL;
            if (endDate.HasValue == false)
                endDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(url))
                return new List<CheckDTO>();

            var sanitizedUrl = SanitizeSiteUrl(url);
            var query = $"SELECT * FROM root.netvigia.{sanitizedUrl} " +
                       $"WHERE time >= {ConvertToUnixTimestamp(startDate)} AND time <= {ConvertToUnixTimestamp(endDate.Value)}";

            return await _repository.ListChecks(query, url);
        }

        private long ConvertToUnixTimestamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public async Task<double> GetUptimePercentageAsync(Guid serverId, TimeSpan period)
        {
            var startDate = period <= TimeSpan.Zero
                ? DateTime.MinValue
                : DateTime.UtcNow - period;

            var endDate = DateTime.UtcNow;

            var checks = await ListChecks(serverId, startDate, endDate);

            var upChecks = checks.Where(x => x.Up).ToList();
            var failedChecks = checks.Except(upChecks).ToList();
            var uptimePercentage = Math.Round((double)upChecks.Count / checks.Count * 100, 2);

            return uptimePercentage;
        }

        public async Task<double> GetAverageResponseTime(Guid serverId, TimeSpan period)
        {
            var startDate = period <= TimeSpan.Zero
                ? DateTime.MinValue
                : DateTime.UtcNow - period;

            var endDate = DateTime.UtcNow;

            var checks = await ListChecks(serverId, startDate, endDate);

            var upChecks = checks.Where(x => x.Up).ToList();
            var avgResponseTime = upChecks.Average(x => x.ResponseTimeInMs);

            return avgResponseTime;
        }

        public async Task<List<CheckDTO>> GetAverageResponseTimeByDate(Guid serverId, TimeSpan period)
        {
            var ret = new List<CheckDTO>();
            var startDate = period <= TimeSpan.Zero
                ? DateTime.MinValue
                : DateTime.UtcNow - period;

            var endDate = DateTime.UtcNow;

            var checks = await ListChecks(serverId, startDate, endDate);

            var upChecks = checks.Where(x => x.Up).ToList();

            var averageByTimestamp = upChecks.GroupBy(dp => dp.Timestamp.Date)
                                        .Select(g => new CheckDTO
                                        {
                                            Timestamp = g.Key,
                                            ResponseTimeInMs = g.Average(dp => dp.ResponseTimeInMs)
                                        }).ToList();

            return averageByTimestamp;
        }

        public async Task<List<CheckDTO>> ListFailedChecks(Guid serverId, DateTime startDate, DateTime? endDate)
        {

            var server = await _serverRepository.GetByIdAsync(serverId);
            if (server == null)
                return new List<CheckDTO>();

            string url = server.URL;
            if (endDate.HasValue == false)
                endDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(url))
                return new List<CheckDTO>();

            var sanitizedUrl = SanitizeSiteUrl(url);
            var query = $"SELECT * FROM root.netvigia.{sanitizedUrl} " +
                       $"WHERE time >= {ConvertToUnixTimestamp(startDate)} AND time <= {ConvertToUnixTimestamp(endDate.Value)}";

            var checks = await _repository.ListChecks(query, url);

            return checks.Where(x => x.Up == false).ToList();
        }

    }
}
