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
    public class SaveTabelaGeralCommandHandler : IRequestHandler<SaveTabelaGeralCommand>
    {
        private readonly ITabelaGeralService _tabelaGeralService;

        public SaveTabelaGeralCommandHandler(ITabelaGeralService tabelaGeralService)
        {
            _tabelaGeralService = tabelaGeralService;
        }

        public async Task Handle(SaveTabelaGeralCommand request, CancellationToken cancellationToken)
        {
            if (request.dto.Id.HasValue)
                await _tabelaGeralService.UpdateAsync(request.dto);
            else
                await _tabelaGeralService.AddAsync(request.dto);
        }
    }
}
