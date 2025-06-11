using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.CommandHandler
{
    public class SaveServerCommandHandler : IRequestHandler<SaveServerCommand>
    {
        private readonly IServerService _serverService;
        public SaveServerCommandHandler(IServerService serverService)
        {
            _serverService = serverService;
        }
        public Task Handle(SaveServerCommand request, CancellationToken cancellationToken)
        {
            if (request.dto.Id.HasValue)
            {
                return _serverService.UpdateAsync(request.dto);
            }
            else
            {
                return _serverService.AddAsync(request.dto);
            }
        }
    }
}
