using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query.Checks;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.QueryHandler.Checks
{
    public class GetChecksByDateQueryHandler : IRequestHandler<GetChecksByDateQuery, List<CheckDTO>>
    {

        private readonly IIoTDBService _service;

        public GetChecksByDateQueryHandler(IIoTDBService service)
        {
            _service = service;
        }

        public async Task<List<CheckDTO>> Handle(GetChecksByDateQuery request, CancellationToken cancellationToken)
        {
            var checks = await _service.ListChecks(request.serverId, request.startDate, request.endDate, request.userId);
            return checks;
        }
    }
}
