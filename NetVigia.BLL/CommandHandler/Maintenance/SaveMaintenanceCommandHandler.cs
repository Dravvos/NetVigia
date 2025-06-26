using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command.Maintenance;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.CommandHandler.Maintenance
{
    public class SaveMaintenanceCommandHandler : IRequestHandler<SaveMaintenanceCommand>
    {
        private readonly IMaintenanceService _maintenanceService;
        public SaveMaintenanceCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task Handle(SaveMaintenanceCommand request, CancellationToken cancellationToken)
        {
            if(request.dto.Id.HasValue)
            {
                await _maintenanceService.UpdateMaintenanceAsync(request.dto);
            }
            else
            {
                await _maintenanceService.CreateMaintenanceAsync(request.dto);
            }
        }
    }
}
