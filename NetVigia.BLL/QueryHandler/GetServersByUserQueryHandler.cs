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
    public class GetServersByUserQueryHandler : IRequestHandler<GetServersByUserQuery, List<ServerDTO>>
    {

        private readonly IServerService _service;
        public GetServersByUserQueryHandler(IServerService service)
        {
            _service = service;
        }
        public async Task<List<ServerDTO>> Handle(GetServersByUserQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(request.userId);
        }
    }
}
