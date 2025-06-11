using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.Data.TimeSeries;

namespace NetVigia.BLL.CommandHandler
{
    public class AddSiteCheckCommandHandler : IRequestHandler<AddSiteCheckCommand, bool>
    {
        private readonly IIoTDBService _service;

        public AddSiteCheckCommandHandler(IIoTDBService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(AddSiteCheckCommand request, CancellationToken cancellationToken)
        {
            return await _service.Insert(request.dto);
        }
    }
}
