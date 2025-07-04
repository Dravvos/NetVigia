using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query.Maintenance;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.QueryHandler.Maintenance
{
    public class GetMaintenanceByUserQueryHandler : IRequestHandler<GetMaintenanceByUserQuery, List<MaintenanceDTO>>
    {
        private readonly IMaintenanceService _maintenanceService;
        public GetMaintenanceByUserQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task<List<MaintenanceDTO>> Handle(GetMaintenanceByUserQuery request, CancellationToken cancellationToken)
        {
            return await _maintenanceService.GetAllMaintenanceAsync(request.userId);
        }
    }
}
