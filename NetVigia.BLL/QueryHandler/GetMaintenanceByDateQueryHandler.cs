using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.QueryHandler
{
    public class GetMaintenanceByDateQueryHandler : IRequestHandler<GetMaintenanceByDateQuery, List<MaintenanceDTO>>
    {
        private readonly IMaintenanceService _maintenanceService;
        public GetMaintenanceByDateQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task<List<MaintenanceDTO>> Handle(GetMaintenanceByDateQuery request, CancellationToken cancellationToken)
        {
            return await _maintenanceService.GetByDateAsync(request.serverId, request.userId, request.startDate, request.endDate);
        }
    }
}
