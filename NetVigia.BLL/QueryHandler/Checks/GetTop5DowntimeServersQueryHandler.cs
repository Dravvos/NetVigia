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
    public class GetTop5DowntimeServersQueryHandler : IRequestHandler<GetTop5DowntimeServersQuery, List<ServerDTO>>
    {
        private readonly IIoTDBService _ioTDBService;

        public GetTop5DowntimeServersQueryHandler(IIoTDBService ioTDBService)
        {
            _ioTDBService = ioTDBService;
        }

        public async Task<List<ServerDTO>> Handle(GetTop5DowntimeServersQuery request, CancellationToken cancellationToken)
        {
            return await _ioTDBService.GetTop5DowntimeServers(request.userId, request.startDate, request.endDate);
        }
    }
}
