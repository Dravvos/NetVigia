using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Services.Interfaces
{
    public interface ICheckService
    {
        Task<CheckDTO> PerformCheckAsync(ServerDTO website);
    }
}
