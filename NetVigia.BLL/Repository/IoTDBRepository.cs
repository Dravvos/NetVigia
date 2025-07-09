using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.IoTDB.DataStructure;
using Apache.IoTDB;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.DTO;
using NetVigia.Data.TimeSeries;
using Microsoft.Extensions.Logging;

namespace NetVigia.BLL.Repository
{
    public class IoTDBRepository : IIoTDBRepository
    {
        private readonly SessionPool.Builder sessionPoolBuilder;
        private readonly IServerRepository _serverRepository;
        private readonly SessionPool sessionPool;

        public IoTDBRepository(IServerRepository serverRepository)
        {
            var dal = new BaseDal();
            sessionPool = dal.sessionPool;
            sessionPoolBuilder = dal.sessionPoolBuilder;
            _serverRepository = serverRepository;
        }

        private CheckDTO? ParseRowRecord(RowRecord rowRecord, string? siteUrl)
        {
            try
            {
                var timestamp = rowRecord.GetDateTime();
                var fields = rowRecord.Values;

                if (fields.Count < 3) return null;

                bool.TryParse(fields[0].ToString(), out bool up);
                int.TryParse(fields[1].ToString(), out int statusCode);
                float.TryParse(fields[2].ToString(), out float latency);

                var dto = new CheckDTO
                {
                    Timestamp = timestamp,
                    StatusCode = statusCode,
                    ResponseTimeInMs = latency,
                    Up = up
                };
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }


        private DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
        }

        public async Task<List<CheckDTO>> ListChecks(string query, string? url)
        {
            var result = new List<CheckDTO>();

            await sessionPool.Open(false);
            if (sessionPool.IsOpen())
            {
                var dataSet = await sessionPool.ExecuteQueryStatementAsync(query);
                await sessionPool.Close();
                sessionPool.Dispose();
                while (dataSet.HasNext())
                {
                    var row = dataSet.Next();
                    var timestamp = row.GetDateTime();
                    var dto = ParseRowRecord(row, url);
                    if (dto != null)
                        result.Add(dto);
                }

                var server = await _serverRepository.GetByUrl(url);
                if (server != null)
                {
                    foreach (var item in result)
                    {
                        item.Server = server;
                        item.ServerId = server.Id.GetValueOrDefault();
                    }
                }
            }
            return result;
        }

        public async Task<bool> InsertCheck(List<object> values, List<string> measurements, DateTime timestamp, List<TSDataType> dataTypes, string url)
        {
            int ret = 0;
            var session = sessionPoolBuilder.Build();
            await session.Open(false);            
            if (session.IsOpen())
            {
                var rowRecord = new RowRecord(timestamp, values, measurements, dataTypes);
                ret = await session.InsertRecordAsync(url, rowRecord);
                if (ret != 0)
                {
                    int ret1 = await session.CreateTimeSeries(url + "." + measurements[0], TSDataType.INT32, TSEncoding.TS_2DIFF, Compressor.LZ4);
                    int ret2 = await session.CreateTimeSeries(url + "." + measurements[1], TSDataType.FLOAT, TSEncoding.GORILLA, Compressor.LZ4);
                    int ret3 = await session.CreateTimeSeries(url + "." + measurements[2], TSDataType.BOOLEAN, TSEncoding.RLE, Compressor.LZ4);
                    for (int i = 0; i < measurements.Count; i++)
                    {
                        ret = await session.InsertRecordAsync(url + "." + measurements[i], rowRecord);
                        if (ret != 0)
                            return false;
                    }

                }
                await session.Close();
            }
            if (ret == 0)
                return true;

            return false;
        }
    }
}
