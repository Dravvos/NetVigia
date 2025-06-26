using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command.Integration;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.CommandHandler.Integration
{
    public class DeleteIntegrationCommandHandler : IRequestHandler<DeleteIntegrationCommand>
    {
        private readonly IIntegrationService _integrationService;
        public DeleteIntegrationCommandHandler(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }
        public async Task Handle(DeleteIntegrationCommand request, CancellationToken cancellationToken)
        {
                await _integrationService.DeleteAsync(request.id);
         
        }
    }
}
