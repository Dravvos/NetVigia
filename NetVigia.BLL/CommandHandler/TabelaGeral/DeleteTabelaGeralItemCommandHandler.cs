using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Command.TabelaGeral;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.CommandHandler.TabelaGeral
{
    public class DeleteTabelaGeralItemCommandHandler : IRequestHandler<DeleteTabelaGeralItemCommand>
    {
        private readonly ITabelaGeralItemService _service;
        public DeleteTabelaGeralItemCommandHandler(ITabelaGeralItemService service)
        {
            _service = service;
        }
        public async Task Handle(DeleteTabelaGeralItemCommand request, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(request.id);
        }
    }
}
