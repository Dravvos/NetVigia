using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Command.TabelaGeral
{
    public record SaveTabelaGeralCommand(TabelaGeralDTO dto) : IRequest;
}
