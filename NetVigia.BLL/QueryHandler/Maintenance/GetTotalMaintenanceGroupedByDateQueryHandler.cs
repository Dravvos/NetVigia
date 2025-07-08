using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query.Maintenance;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.QueryHandler.Maintenance
{
    public class GetTotalMaintenanceGroupedByDateQueryHandler : IRequestHandler<GetTotalMaintenanceGroupedByDateQuery, Dictionary<string, int>>
    {
        private readonly IMaintenanceService _maintenanceService;
        public GetTotalMaintenanceGroupedByDateQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task<Dictionary<string, int>> Handle(GetTotalMaintenanceGroupedByDateQuery request, CancellationToken cancellationToken)
        {
            return await _maintenanceService.GetGroupedByDateAsync(request.groupingType, request.serverIds, request.startDate, request.endDate);
        }
    }
}
