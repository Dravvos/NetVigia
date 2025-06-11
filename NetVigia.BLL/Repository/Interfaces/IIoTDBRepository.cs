using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.IoTDB;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository.Interfaces
{
    public interface IIoTDBRepository
    {
        Task<bool> InsertCheck(List<object> values, List<string> measurements, DateTime timestamp, List<TSDataType> dataTypes, string url);
        Task<List<CheckDTO>> ListChecks(string query, string? url);
    }
}
