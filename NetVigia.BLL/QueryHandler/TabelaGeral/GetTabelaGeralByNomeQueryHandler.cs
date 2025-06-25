using MediatR;
using NetVigia.BLL.Query.TabelaGeral;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.QueryHandler.TabelaGeral
{
    public class GetTabelaGeralByNomeQueryHandler : IRequestHandler<GetTabelaGeralByNomeQuery, TabelaGeralDTO>
    {
        private readonly ITabelaGeralService _tabelaGeralService;

        public GetTabelaGeralByNomeQueryHandler(ITabelaGeralService tabelaGeralService)
        {
            _tabelaGeralService = tabelaGeralService;
        }

        public async Task<TabelaGeralDTO> Handle(GetTabelaGeralByNomeQuery request, CancellationToken cancellationToken)
        {
            return await _tabelaGeralService.GetByNomeAsync(request.nome);
        }
    }
}
