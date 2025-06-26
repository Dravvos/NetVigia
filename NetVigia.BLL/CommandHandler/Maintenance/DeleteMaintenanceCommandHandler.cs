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
    public class DeleteMaintenanceCommandHandler : IRequestHandler<DeleteMaintenanceCommand>
    {
        private readonly IMaintenanceService _maintenanceService;
        public DeleteMaintenanceCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        public async Task Handle(DeleteMaintenanceCommand request, CancellationToken cancellationToken)
        {
            await _maintenanceService.DeleteMaintenanceAsync(request.id);
        }
    }
}
