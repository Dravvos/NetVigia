using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NetVigia.BLL.Command.Integration
{
    public record DeleteIntegrationCommand(Guid id):IRequest<bool>;
}
