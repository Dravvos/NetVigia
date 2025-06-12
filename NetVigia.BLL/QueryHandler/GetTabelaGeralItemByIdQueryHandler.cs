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
    public class GetTabelaGeralItemByIdQueryHandler : IRequestHandler<GetTabelaGeralItemByIdQuery, TabelaGeralItemDTO>
    {
        private readonly ITabelaGeralItemService _service;
        public GetTabelaGeralItemByIdQueryHandler(ITabelaGeralItemService service)
        {
            _service = service;
        }
        public async Task<TabelaGeralItemDTO> Handle(GetTabelaGeralItemByIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(request.id);
        }
    }
}
