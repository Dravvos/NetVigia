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
    public class GetAverageMaintenanceDurationQueryHandler : IRequestHandler<GetAverageMaintenanceDurationQuery, TimeSpan>
    {
        private readonly IMaintenanceService _maintenanceService;
        public GetAverageMaintenanceDurationQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task<TimeSpan> Handle(GetAverageMaintenanceDurationQuery request, CancellationToken cancellationToken)
        {
            return await _maintenanceService.GetAverageMaintenanceDuration(request.startDate, request.endDate, request.serverIds);
        }
    }
}
