using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command.Server;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.CommandHandler.Server
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
                var server =  _serverService.GetById(request.dto.Id.Value).Result;
                if(server.UserId != request.dto.UserId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to update this server.");
                }
                return _serverService.UpdateAsync(request.dto);
            }
            else
            {
                return _serverService.AddAsync(request.dto);
            }
        }
    }
}
