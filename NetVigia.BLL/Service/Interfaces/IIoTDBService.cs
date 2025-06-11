using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Service.Interfaces
{
    public interface IIoTDBService
    {
        Task<bool> Insert(CheckDTO check);
        Task<List<CheckDTO>> ListChecks(string? url, DateTime startDate, DateTime? endDate);
    }
}
