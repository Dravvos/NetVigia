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
    public class DeleteTabelaGeralItemCommandHandler : IRequestHandler<DeleteTabelaGeralItemCommand, bool>
    {
        private readonly ITabelaGeralItemService _service;
        public DeleteTabelaGeralItemCommandHandler(ITabelaGeralItemService service)
        {
            _service = service;
        }
        public async Task<bool> Handle(DeleteTabelaGeralItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeleteAsync(request.id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
