using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.CommandHandler
{
    public class SaveTabelaGeralItemCommandHandler : IRequestHandler<SaveTabelaGeralItemCommand>
    {
        private readonly ITabelaGeralItemService _service;

        public SaveTabelaGeralItemCommandHandler(ITabelaGeralItemService service)
        {
            _service = service;
        }

        public Task Handle(SaveTabelaGeralItemCommand request, CancellationToken cancellationToken)
        {
            if (request.dto.Id.HasValue)          
                return _service.UpdateAsync(request.dto);
            
            else
                return _service.AddAsync(request.dto);
            
        }
    }
}
