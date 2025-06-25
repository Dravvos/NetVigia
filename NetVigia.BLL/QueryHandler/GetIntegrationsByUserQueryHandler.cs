using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.QueryHandler
{
    public class GetIntegrationsByUserQueryHandler : IRequestHandler<GetIntegrationsByUserQuery, List<IntegrationDTO>>
    {
        private readonly IIntegrationService _integrationService;
        public GetIntegrationsByUserQueryHandler(IIntegrationService integrationService)
        {
            _integrationService = integrationService ?? throw new ArgumentNullException(nameof(integrationService));
        }
        public async Task<List<IntegrationDTO>> Handle(GetIntegrationsByUserQuery request, CancellationToken cancellationToken)
        {
            return await _integrationService.GetByUserAsync(request.userId);
        }
    }
}
