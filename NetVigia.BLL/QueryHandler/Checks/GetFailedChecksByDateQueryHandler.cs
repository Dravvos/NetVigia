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
    public class GetFailedChecksByDateQueryHandler : IRequestHandler<GetFailedChecksByDateQuery, List<CheckDTO>>
    {
        private readonly IIoTDBService _service;
        public GetFailedChecksByDateQueryHandler(IIoTDBService service)
        {
            _service = service;
        }
        public async Task<List<CheckDTO>> Handle(GetFailedChecksByDateQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetFailedChecksByDate(request.serverId, request.startDate, request.endDate);
        }
    }
}
