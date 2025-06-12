using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NetVigia.BLL.Command.Server
{
    public record DeleteServerCommand(Guid Id) : IRequest<bool>;
}
