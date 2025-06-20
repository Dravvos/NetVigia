using MediatR;
using NetVigia.BLL.Query;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.QueryHandler
{
    public class GetFailedChecksByDateQueryHandler : IRequestHandler<GetFailedChecksByDateQuery, List<CheckDTO>>
    {
        private readonly IIoTDBService _service;

        public GetFailedChecksByDateQueryHandler(IIoTDBService service)
        {
            _service = service;
        }
        public async Task<List<CheckDTO>> Handle(GetFailedChecksByDateQuery request, CancellationToken cancellationToken)
        {
            return await _service.ListFailedChecks(request.serverId, request.startDate, request.endDate);
        }
    }
}
