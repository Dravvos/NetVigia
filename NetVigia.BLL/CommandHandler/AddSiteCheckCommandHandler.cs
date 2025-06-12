using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.BLL.Services.Interfaces;
using NetVigia.Data.TimeSeries;
using NetVigia.DTO;

namespace NetVigia.BLL.CommandHandler
{
    public class AddSiteCheckCommandHandler : IRequestHandler<AddSiteCheckCommand, CheckDTO>
    {
        private readonly ICheckService _checkService;
        private readonly IIoTDBService _ioTDBService;
        private readonly IServerRepository _serverRepository;

        public AddSiteCheckCommandHandler(ICheckService checkService, IIoTDBService ioTDBService, IServerRepository serverRepository)
        {
            _checkService = checkService;
            _ioTDBService = ioTDBService;
            _serverRepository = serverRepository;
        }

        public async Task<CheckDTO> Handle(AddSiteCheckCommand request, CancellationToken cancellationToken)
        {
            var serverDTO = await _serverRepository.GetByIdAsync(request.serverId);
            if (serverDTO == null)
                throw new KeyNotFoundException($"Server with ID {request.serverId} not found.");
            var result = await _checkService.PerformCheckAsync(serverDTO);
            
            result.Server = serverDTO;

            await _ioTDBService.Insert(result);

            return result;
        }
    }
}
