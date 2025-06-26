using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NetVigia.BLL.Command.Maintenance
{
    public record DeleteMaintenanceCommand(Guid id):IRequest;
}
