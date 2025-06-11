using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.Data.TimeSeries;
using NetVigia.DTO;

namespace NetVigia.BLL.Command
{
    public record AddSiteCheckCommand(CheckDTO dto) : IRequest<bool>;

}
