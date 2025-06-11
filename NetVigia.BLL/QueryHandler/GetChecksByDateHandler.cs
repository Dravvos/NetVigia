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
    public class GetChecksByDateHandler : IRequestHandler<GetChecksByDate, List<CheckDTO>>
    {

        private readonly IIoTDBService _service;

        public GetChecksByDateHandler(IIoTDBService service)
        {
            _service = service;
        }

        public async Task<List<CheckDTO>> Handle(GetChecksByDate request, CancellationToken cancellationToken)
        {
            var checks = await _service.ListChecks(request.url, request.startDate, request.endDate);
            return checks;
        }
    }
}
