using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class TabelaGeralItemDTO : BaseDTO
    {
        public Guid TabelaGeralId { get; set; }
        public string? Sigla { get; set; }
        public string? Descricao { get; set; }
        public TabelaGeralDTO? TabelaGeral { get; set; }
    }
}
