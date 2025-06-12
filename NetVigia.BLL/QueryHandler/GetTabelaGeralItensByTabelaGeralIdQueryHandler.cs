using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.QueryHandler
{
    public class GetTabelaGeralItensByTabelaGeralIdQueryHandler : IRequestHandler<GetTabelaGeralItensByTabelaGeralIdQuery, List<TabelaGeralItemDTO>>
    {
        private readonly ITabelaGeralItemService _service;

        public GetTabelaGeralItensByTabelaGeralIdQueryHandler(ITabelaGeralItemService service)
        {
            _service = service;
        }

        public async Task<List<TabelaGeralItemDTO>> Handle(GetTabelaGeralItensByTabelaGeralIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllItemsAsync(request.tabelaGeralId);
        }
    }
}
