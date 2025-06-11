using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class TabelaGeralDTO : BaseDTO
    {
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
    }
}
