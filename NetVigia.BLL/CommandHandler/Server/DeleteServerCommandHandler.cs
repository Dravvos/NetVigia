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
    public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, bool>
    {
        private readonly IServerService _serverService;
        public DeleteServerCommandHandler(IServerService serverService)
        {
            _serverService = serverService;
        }
        public async Task<bool> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _serverService.DeleteAsync(request.Id);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
