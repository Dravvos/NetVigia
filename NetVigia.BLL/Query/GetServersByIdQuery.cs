using MediatR;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.Query
{
    public record GetServersByIdQuery(Guid id) : IRequest<ServerDTO>;
}
