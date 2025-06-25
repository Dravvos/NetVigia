using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Command.Integration
{
    public record SaveIntegrationCommand(IntegrationDTO dto) : IRequest;
}
