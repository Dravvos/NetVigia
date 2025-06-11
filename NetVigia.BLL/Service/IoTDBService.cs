using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.IoTDB;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Service
{
    public class IoTDBService : IIoTDBService
    {
        private readonly IIoTDBRepository _repository;

        public IoTDBService(IIoTDBRepository repository)
        {
            _repository = repository;
        }
        private string SanitizeSiteUrl(string url) =>
      url.Replace("https://", "")
         .Replace("http://", "")
         .Replace("/", "_")
         .Replace(".", "_")
         .Replace(":", "_");

        public async Task<bool> Insert(CheckDTO check)
        {
            string url = $"root.netvigia.{SanitizeSiteUrl(check.URL)}";

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
                check.Latency,
                check.Up
            };
            return await _repository.InsertCheck(values, measurements, check.Timestamp, dataTypes, url);
        }

        public async Task<List<CheckDTO>> ListChecks(string? url, DateTime startDate, DateTime? endDate)
        {

            if (endDate.HasValue == false)
                endDate = DateTime.Now;
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

    }
}
