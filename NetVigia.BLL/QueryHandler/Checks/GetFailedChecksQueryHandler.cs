using MediatR;
using NetVigia.BLL.Query.Checks;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.QueryHandler.Checks
{
    public class GetFailedChecksQueryHandler : IRequestHandler<GetFailedChecksQuery, List<CheckDTO>>
    {
        private readonly IIoTDBService _service;

        public GetFailedChecksQueryHandler(IIoTDBService service)
        {
            _service = service;
        }
        public async Task<List<CheckDTO>> Handle(GetFailedChecksQuery request, CancellationToken cancellationToken)
        {
            return await _service.ListFailedChecks(request.serverId, request.startDate, request.endDate);
        }
    }
}
