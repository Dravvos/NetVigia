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
    public class GetAverageResponseTimeByDateQueryHandler : IRequestHandler<GetAverageResponseTimeByDateQuery, List<CheckDTO>>
    {
        private readonly IIoTDBService _service;

        public GetAverageResponseTimeByDateQueryHandler(IIoTDBService service)
        {
            _service = service;
        }

        public async Task<List<CheckDTO>> Handle(GetAverageResponseTimeByDateQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAverageResponseTimeByDate(request.serverId, request.period);
        }
    }
}
