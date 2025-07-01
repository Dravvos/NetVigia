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
    public class GetTabelaGeralItemQueryHandler : IRequestHandler<GetTabelaGeralItemQuery, TabelaGeralItemDTO>
    {
        private readonly ITabelaGeralItemService _tabelaGeralItemService;

        public GetTabelaGeralItemQueryHandler(ITabelaGeralItemService tabelaGeralItemService)
        {
            _tabelaGeralItemService = tabelaGeralItemService;
        }

        public async Task<TabelaGeralItemDTO> Handle(GetTabelaGeralItemQuery request, CancellationToken cancellationToken)
        {
            return await _tabelaGeralItemService.GetBySiglaAsync(request.tabelaGeralId, request.sigla);
        }
    }
}
