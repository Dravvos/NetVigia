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
    public class DeleteIntegrationCommandHandler : IRequestHandler<DeleteIntegrationCommand, bool>
    {
        private readonly IIntegrationService _integrationService;
        public DeleteIntegrationCommandHandler(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }
        public async Task<bool> Handle(DeleteIntegrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _integrationService.DeleteAsync(request.id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
