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
    public class SaveIntegrationCommandHandler : IRequestHandler<SaveIntegrationCommand>
    {
        private readonly IIntegrationService _integrationService;
        public SaveIntegrationCommandHandler(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }
        public async Task Handle(SaveIntegrationCommand request, CancellationToken cancellationToken)
        {
            if (request.dto.Id.HasValue)
            {
                var existingIntegration = await _integrationService.GetByUserAsync(request.dto.UserId);
                if (existingIntegration == null)
                    throw new KeyNotFoundException("Integration not found for the given user.");
                await _integrationService.UpdateAsync(request.dto); 
            }
            else
                await _integrationService.CreateAsync(request.dto);
        }
    }
}
