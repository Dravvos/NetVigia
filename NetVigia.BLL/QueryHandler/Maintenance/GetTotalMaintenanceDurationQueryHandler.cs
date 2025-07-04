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
    public class GetTotalMaintenanceDurationQueryHandler : IRequestHandler<GetTotalMaintenanceDurationQuery, TimeSpan>
    {
        private readonly IMaintenanceService _maintenanceService;
        public GetTotalMaintenanceDurationQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task<TimeSpan> Handle(GetTotalMaintenanceDurationQuery request, CancellationToken cancellationToken)
        {
            return await _maintenanceService.GetTotalMaintenanceDuration(request.startDate, request.endDate, request.serverIds);
        }
    }
}
