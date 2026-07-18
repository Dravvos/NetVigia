using MediatR;
using NetVigia.BLL.Query.Maintenance;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.QueryHandler.Maintenance
{
    public class GetMaintenanceByIdQueryHandler : IRequestHandler<GetMaintenanceByIdQuery,MaintenanceDTO>
    {
        private readonly IMaintenanceService _maintenanceService;

        public GetMaintenanceByIdQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public Task<MaintenanceDTO> Handle(GetMaintenanceByIdQuery request, CancellationToken cancellationToken)
        {
            return _maintenanceService.GetMaintenanceByIdAsync(request.id);
        }
    }
}
