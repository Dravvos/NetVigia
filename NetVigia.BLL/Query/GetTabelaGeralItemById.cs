using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Query
{
    public record GetTabelaGeralItemById(Guid id) : IRequest<TabelaGeralItemDTO>;
}
