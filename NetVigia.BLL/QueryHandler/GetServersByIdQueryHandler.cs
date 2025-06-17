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
    public class GetServersByIdQueryHandler : IRequestHandler<GetServersByIdQuery, ServerDTO>
    {
        private readonly IServerService _serverService;

        public GetServersByIdQueryHandler(IServerService serverService)
        {
            _serverService = serverService;
        }

        public async Task<ServerDTO> Handle(GetServersByIdQuery request, CancellationToken cancellationToken)
        {
            return await _serverService.GetById(request.id);
        }
    }
}
