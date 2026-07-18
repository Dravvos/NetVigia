using MediatR;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.Query.Maintenance
{
    public record GetMaintenanceByIdQuery(Guid id):IRequest<MaintenanceDTO>;
}
